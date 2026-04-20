using Microsoft.EntityFrameworkCore;
using Temsa.Core.Application.Abstractions.Time;
using Temsa.Core.Application.Scans.Abstractions;
using Temsa.Core.Application.Scans.Models;
using Temsa.Core.Application.Scans.Services;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Domain.Enums;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Scans.Commands.StartScan;

public class StartScanHandler(
    TemsaDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IScanTaskPublisher scanTaskPublisher,
    ScanStatusCalculator scanStatusCalculator,
    ILogger<StartScanHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IScanTaskPublisher _scanTaskPublisher = scanTaskPublisher;
    private readonly ScanStatusCalculator _scanStatusCalculator = scanStatusCalculator;
    private readonly ILogger<StartScanHandler> _logger = logger;

    public async Task<StartScanResult> HandleAsync(
        StartScanCommand command,
        CancellationToken cancellationToken = default)
    {
        var scan = await _dbContext.Scans
            .Include(x => x.Tasks)
            .Include(x => x.Events)
            .FirstOrDefaultAsync(x => x.Id == command.ScanId, cancellationToken);

        if (scan is null)
        {
            throw new InvalidOperationException($"Scan with id '{command.ScanId}' was not found");
        }

        if (scan.Status != ScanStatus.Pending)
        {
            throw new InvalidOperationException(
                $"Scan with id '{command.ScanId}' cannot be started from status '{scan.Status}'");
        }

        var pendingTasks = scan.Tasks
            .Where(x => x.Status == ScanTaskStatus.Pending)
            .OrderBy(x => x.Order)
            .ToArray();

        if (pendingTasks.Length == 0)
        {
            throw new InvalidOperationException(
                $"Scan with id '{command.ScanId}' does not contain pending tasks");
        }
        
        _logger.LogDebug(
            "Starting scan {ScanId} with {TaskCount} pending tasks",
            scan.Id,
            pendingTasks.Length);
        
        foreach (var task in pendingTasks)
        {
            var message = new ScanTaskDispatchMessage(
                ScanTaskId: task.Id,
                ScanId: scan.Id,
                InputArtifactId: scan.InputArtifactId,
                Platform: scan.Platform,
                TaskType: task.TaskType,
                Tool: task.Tool,
                ParametersJson: task.PayloadJson);

            await _scanTaskPublisher.PublishAsync(task.WorkerType, message, cancellationToken);
            
            task.Status = ScanTaskStatus.Queued;
            task.UpdatedAt = _dateTimeProvider.UtcNow;
        }

        scan.Status = _scanStatusCalculator.Calculate(scan.Tasks.Select(x => x.Status).ToArray());
        scan.CurrentStage = "queued";
        scan.UpdatedAt = _dateTimeProvider.UtcNow;

        scan.Events.Add(new ScanEvent
        {
            EventType = "scan.started",
            PayloadJson = null,
            CreatedAt = _dateTimeProvider.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Scan {ScanId} started successfully. {TaskCount} tasks queued",
            scan.Id,
            pendingTasks.Length);

        return new StartScanResult(
            ScanId: scan.Id,
            Status: scan.Status,
            QueuedTaskCount: pendingTasks.Length,
            UpdatedAt: scan.UpdatedAt);
    }
}