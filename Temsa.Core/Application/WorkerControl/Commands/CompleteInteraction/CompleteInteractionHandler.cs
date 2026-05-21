using Microsoft.EntityFrameworkCore;
using Temsa.Common.Time;
using Temsa.Contracts.Messaging.WorkerControl;
using Temsa.Core.Application.WorkerControl.Abstractions;
using Temsa.Core.Domain.Enums;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.WorkerControl.Commands.CompleteInteraction;

public class CompleteInteractionHandler(
    TemsaDbContext dbContext,
    IWorkerControlPublisher publisher,
    IDateTimeProvider dateTimeProvider,
    ILogger<CompleteInteractionHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IWorkerControlPublisher _publisher = publisher;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<CompleteInteractionHandler> _logger = logger;

    public async Task<CompleteInteractionResult> HandleAsync(
        CompleteInteractionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.ScanTaskId is not null)
        {
            return await CompleteTaskInteractionAsync(
                command.ScanId,
                command.ScanTaskId.Value,
                cancellationToken);
        }

        return await CompleteScanInteractionAsync(
            command.ScanId,
            cancellationToken);
    }
    
    private static bool IsDynamicWorker(string workerType)
    {
        return string.Equals(workerType, "android-dynamic-analysis", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(workerType, "ios-dynamic-analysis", StringComparison.OrdinalIgnoreCase);
    }
    
    private async Task<CompleteInteractionResult> CompleteTaskInteractionAsync(
        long scanId,
        long scanTaskId,
        CancellationToken cancellationToken)
    {
        var scanTask = await _dbContext.ScanTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == scanTaskId &&
                     x.ScanId == scanId,
                cancellationToken);

        if (scanTask is null)
        {
            throw new InvalidOperationException(
                $"Scan task with id '{scanTaskId}' was not found in scan '{scanId}'");
        }

        if (scanTask.Status != ScanTaskStatus.Running)
        {
            throw new InvalidOperationException(
                $"Scan task '{scanTaskId}' cannot complete interaction from status '{scanTask.Status}'");
        }

        if (!IsDynamicWorker(scanTask.WorkerType))
        {
            throw new InvalidOperationException(
                $"Scan task '{scanTaskId}' is not an interactive dynamic analysis task");
        }

        var occuredAt = _dateTimeProvider.UtcNow;

        var message = new WorkerControlMessage(
            ScanId: scanId,
            ScanTaskId: scanTaskId,
            CommandType: WorkerControlCommandTypes.InteractionCompleted,
            Reason: "Interaction completed by user",
            OccuredAt: occuredAt);

        await _publisher.PublishAsync(
            routingKey: scanTask.WorkerType,
            message,
            cancellationToken);

        _logger.LogInformation(
            "Interaction completion command published for scan {ScanId}, task {ScanTaskId}",
            scanId,
            scanTaskId);

        return new CompleteInteractionResult(
            scanId,
            scanTaskId,
            message.CommandType,
            occuredAt);
    }
    
    private async Task<CompleteInteractionResult> CompleteScanInteractionAsync(
        long scanId,
        CancellationToken cancellationToken)
    {
        var runningDynamicTasks = await _dbContext.ScanTasks
            .AsNoTracking()
            .Where(x =>
                x.ScanId == scanId &&
                x.Status == ScanTaskStatus.Running)
            .ToArrayAsync(cancellationToken);

        if (runningDynamicTasks.Length == 0)
        {
            var scanExists = await _dbContext.Scans
                .AsNoTracking()
                .AnyAsync(x => x.Id == scanId, cancellationToken);

            if (!scanExists)
            {
                throw new InvalidOperationException(
                    $"Scan with id '{scanId}' was not found");
            }

            throw new InvalidOperationException(
                $"Scan '{scanId}' has no running interactive dynamic analysis tasks");
        }

        runningDynamicTasks = runningDynamicTasks
            .Where(x => IsDynamicWorker(x.WorkerType))
            .ToArray();

        if (runningDynamicTasks.Length == 0)
        {
            throw new InvalidOperationException(
                $"Scan '{scanId}' has running tasks, but none of them are dynamic analysis tasks");
        }

        var workerTypes = runningDynamicTasks
            .Select(x => x.WorkerType)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var occuredAt = _dateTimeProvider.UtcNow;

        foreach (var workerType in workerTypes)
        {
            var message = new WorkerControlMessage(
                ScanId: scanId,
                ScanTaskId: null,
                CommandType: WorkerControlCommandTypes.InteractionCompleted,
                Reason: "Interaction completed by user",
                OccuredAt: occuredAt);

            await _publisher.PublishAsync(
                routingKey: workerType,
                message,
                cancellationToken);
        }

        _logger.LogInformation(
            "Scan-level interaction completion command published for scan {ScanId}. " +
            "RunningTasks={RunningTasksCount}, WorkerTypes={WorkerTypes}",
            scanId,
            runningDynamicTasks.Length,
            string.Join(", ", workerTypes));

        return new CompleteInteractionResult(
            scanId,
            ScanTaskId: null,
            WorkerControlCommandTypes.InteractionCompleted,
            occuredAt);
    }
}