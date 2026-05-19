using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Temsa.Common.Time;
using Temsa.Contracts.Artifacts;
using Temsa.Contracts.Messaging.WorkerEvents;
using Temsa.Core.Application.Scans.Abstractions;
using Temsa.Core.Application.Scans.Services;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Domain.Enums;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.WorkerEvents.Commands.HandleWorkerEvent;

public class HandleWorkerEventHandler(
    TemsaDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ScanStatusCalculator scanStatusCalculator,
    ScanTaskDispatchOrchestrator dispatchOrchestrator,
    ILogger<HandleWorkerEventHandler> logger)
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ScanStatusCalculator _scanStatusCalculator = scanStatusCalculator;
    private readonly ScanTaskDispatchOrchestrator _dispatchOrchestrator = dispatchOrchestrator;
    private readonly ILogger<HandleWorkerEventHandler> _logger = logger;

    public async Task<HandleWorkerEventResult> HandleAsync(
        HandleWorkerEventCommand command,
        CancellationToken cancellationToken = default)
    {
        var message = command.Message;

        _logger.LogInformation(
            "Handling worker event {EventType} for scan {ScanId}, task {ScanTaskId}, worker {WorkerId}, attempt {Attempt}",
            message.EventType,
            message.ScanId,
            message.ScanTaskId,
            message.WorkerId,
            message.Attempt);

        var scan = await _dbContext.Scans
            .Include(x => x.Tasks)
            .Include(x => x.Events)
            .Include(x => x.Artifacts)
            .Include(x => x.InputArtifact)
            .FirstOrDefaultAsync(x => x.Id == message.ScanId, cancellationToken);

        if (scan is null)
        {
            throw new InvalidOperationException($"Scan with id '{message.ScanId}' was not found");
        }

        var scanTask = scan.Tasks.FirstOrDefault(x => x.Id == message.ScanTaskId);

        if (scanTask is null)
        {
            throw new InvalidOperationException(
                $"Scan task with id '{message.ScanTaskId}' was not found in scan '{message.ScanId}'");
        }

        if (scanTask.Attempt != message.Attempt)
        {
            _logger.LogWarning(
                "Worker event attempt mismatch for scan task {ScanTaskId}. " +
                "Current attempt {CurrentAttempt}, event attempt {EventAttempt}",
                scanTask.Id,
                scanTask.Attempt,
                message.Attempt);

            throw new InvalidOperationException($"Worker event attempt mismatch for scan task '{scanTask.Id}'");
        }

        if (IsTerminal(scanTask.Status))
        {
            _logger.LogWarning(
                "Ignoring worker event {EventType} for terminal scan task {ScanTaskId} with status {Status}",
                message.EventType,
                scanTask.Id,
                scanTask.Status);

            return new HandleWorkerEventResult(
                scan.Id,
                scanTask.Id,
                scan.Status,
                scanTask.Status,
                scan.UpdatedAt);
        }

        switch (message.EventType)
        {
            case WorkerEventTypes.TaskStarted:
                ApplyTaskStarted(scan, scanTask, message);
                break;
            
            case WorkerEventTypes.TaskCompleted:
                ApplyTaskCompleted(scan, scanTask, message);
                break;
            
            case WorkerEventTypes.TaskFailed:
                ApplyTaskFailed(scanTask, message);
                break;
            
            case WorkerEventTypes.TaskProgress:
            case WorkerEventTypes.TaskLog:
                ApplyNonTerminalEvent(scanTask);
                break;
            
            default:
                throw new InvalidOperationException($"Unsupported worker event type '{message.EventType}'");
        }

        if (message.EventType == WorkerEventTypes.TaskCompleted)
        {
            await _dispatchOrchestrator.QueueNextAfterTaskCompletedAsync(scan, scanTask, cancellationToken);
        }
        else if (message.EventType == WorkerEventTypes.TaskFailed)
        {
            await _dispatchOrchestrator.QueueNextAfterTaskFailedAsync(scan, scanTask, cancellationToken);
        }

        scan.Status = _scanStatusCalculator.Calculate(
            scan.Tasks.Select(x => x.Status).ToArray());
        
        UpdateScanFieldsAfterStatusRecalculation(scan, scanTask);
        
        scan.Events.Add(new ScanEvent
        {
            ScanTaskId = message.ScanTaskId,
            EventType = message.EventType,
            PayloadJson = BuildScanEventPayloadJson(message),
            CreatedAt = _dateTimeProvider.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Worker event {EventType} handled successfully. " +
            "Scan {ScanId} status: {ScanStatus}, task {ScanTaskId} status: {ScanTaskStatus}",
            message.EventType,
            scan.Id,
            scan.Status,
            scanTask.Id,
            scanTask.Status);

        return new HandleWorkerEventResult(
            scan.Id,
            scanTask.Id,
            scan.Status,
            scanTask.Status,
            scan.UpdatedAt);
    }

    private static bool IsTerminal(ScanTaskStatus status)
    {
        return status is
            ScanTaskStatus.Completed or
            ScanTaskStatus.Failed or
            ScanTaskStatus.Cancelled;
    }

    private void ApplyTaskStarted(
        Scan scan, // TODO убрать scan?
        ScanTask scanTask,
        WorkerEventMessage message)
    {
        scanTask.Status = ScanTaskStatus.Running;
        scanTask.StartedAt ??= message.OccuredAt == default ? _dateTimeProvider.UtcNow : message.OccuredAt;
        scanTask.UpdatedAt = _dateTimeProvider.UtcNow;

        scan.Status = ScanStatus.Running;
        scan.StartedAt ??= scanTask.StartedAt;
        scan.CurrentStage = scanTask.StageId;
        scan.UpdatedAt = _dateTimeProvider.UtcNow;
    }

    private void ApplyTaskCompleted(
        Scan scan,
        ScanTask scanTask,
        WorkerEventMessage message)
    {
        scanTask.Status = ScanTaskStatus.Completed;
        scanTask.ResultJson = message.Payload?.GetRawText();
        scanTask.FinishedAt = message.OccuredAt == default ? _dateTimeProvider.UtcNow : message.OccuredAt;
        scanTask.UpdatedAt = _dateTimeProvider.UtcNow;

        var completedPayload = DeserializeCompletedPayload(message);

        if (completedPayload is null)
        {
            return;
        }
        
        RegisterScanArtifacts(scan, completedPayload.Artifacts);
    }

    private void ApplyTaskFailed(
        ScanTask scanTask,
        WorkerEventMessage message)
    {
        scanTask.Status = ScanTaskStatus.Failed;
        scanTask.ErrorMessage = message.Message;
        scanTask.ResultJson = message.Payload?.GetRawText();
        scanTask.FinishedAt = message.OccuredAt == default ? _dateTimeProvider.UtcNow : message.OccuredAt;
        scanTask.UpdatedAt = _dateTimeProvider.UtcNow;
    }

    private void ApplyNonTerminalEvent(ScanTask scanTask)
    {
        scanTask.UpdatedAt = _dateTimeProvider.UtcNow;
    }

    private void UpdateScanFieldsAfterStatusRecalculation(
        Scan scan,
        ScanTask scanTask)
    {
        scan.UpdatedAt = _dateTimeProvider.UtcNow;

        switch (scan.Status)
        {
            case ScanStatus.Running:
                scan.StartedAt ??= _dateTimeProvider.UtcNow;
                scan.CurrentStage = scanTask.StageId;
                break;
            
            case ScanStatus.Completed:
                scan.FinishedAt ??= _dateTimeProvider.UtcNow;
                scan.CurrentStage = "completed";
                scan.ErrorMessage = null;
                break;
            
            case ScanStatus.Failed:
                scan.FinishedAt ??= _dateTimeProvider.UtcNow;
                scan.CurrentStage = "failed";
                scan.ErrorMessage ??= "One or more scan tasks failed";
                break;
            
            case ScanStatus.Cancelled:
                scan.FinishedAt ??= _dateTimeProvider.UtcNow;
                scan.CurrentStage = "cancelled";
                break;
            
            case ScanStatus.Queued:
                scan.CurrentStage = "queued";
                break;
            
            case ScanStatus.Pending:
                scan.CurrentStage = "created";
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(scan), scan.Status, "Invalid scan status");
        }
    }

    private static string BuildScanEventPayloadJson(WorkerEventMessage message)
    {
        return JsonSerializer.Serialize(message, JsonSerializerOptions);
    }

    private static WorkerTaskCompletedPayload? DeserializeCompletedPayload(
        WorkerEventMessage message)
    {
        return message.Payload?.Deserialize<WorkerTaskCompletedPayload>(JsonSerializerOptions);
    }

    private void RegisterScanArtifacts(
        Scan scan,
        IReadOnlyCollection<ScanArtifactDescriptor>? artifactDescriptors)
    {
        if (artifactDescriptors is null || artifactDescriptors.Count == 0)
        {
            return;
        }

        foreach (var descriptor in artifactDescriptors)
        {
            if (string.IsNullOrWhiteSpace(descriptor.Bucket) ||
                string.IsNullOrWhiteSpace(descriptor.ObjectKey))
            {
                _logger.LogWarning(
                    "Skipping scan artifact with empty bucket or object key. ScanId={ScanId}",
                    scan.Id);
                
                continue;
            }
            
            _dbContext.ScanArtifacts.Add(new ScanArtifact
            {
                ScanId = scan.Id,
                Kind = descriptor.Kind,
                Bucket = descriptor.Bucket,
                ObjectKey = descriptor.ObjectKey,
                FileName = descriptor.FileName,
                ContentType = descriptor.ContentType,
                SizeBytes = descriptor.SizeBytes,
                CreatedAt = _dateTimeProvider.UtcNow
            });
        }
    }
}