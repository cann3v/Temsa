using Microsoft.EntityFrameworkCore;
using Temsa.Common.Time;
using Temsa.Contracts.Artifacts;
using Temsa.Core.Application.Scans.Abstractions;
using Temsa.Contracts.Messaging.ScanTasks;
using Temsa.Core.Application.Scans.Services;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Domain.Enums;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Scans.Commands.StartScan;

public class StartScanHandler(
    TemsaDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ScanStatusCalculator scanStatusCalculator,
    ScanTaskDispatchOrchestrator dispatchOrchestrator,
    ILogger<StartScanHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ScanStatusCalculator _scanStatusCalculator = scanStatusCalculator;
    private readonly ScanTaskDispatchOrchestrator _dispatchOrchestrator = dispatchOrchestrator;
    private readonly ILogger<StartScanHandler> _logger = logger;

    public async Task<StartScanResult> HandleAsync(
        StartScanCommand command,
        CancellationToken cancellationToken = default)
    {
        var scan = await _dbContext.Scans
            .Include(x => x.Tasks)
            .Include(x => x.Events)
            .Include(x => x.InputArtifact)
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

        var inputArtifact = scan.InputArtifact;

        if (inputArtifact is null)
        {
            throw new InvalidOperationException(
                $"Input artifact with id '{scan.InputArtifactId}' was not found");
        }

        var queuedTaskCount = await _dispatchOrchestrator.QueueInitialStageAsync(scan, cancellationToken);

        if (queuedTaskCount == 0)
        {
            throw new InvalidOperationException(
                $"Scan with id '{scan.Id}' does not contain dispatchable pending tasks");
        }

        scan.Status = _scanStatusCalculator.Calculate(scan.Tasks.Select(x => x.Status).ToArray());

        var currentStage = scan.Tasks
            .Where(x => x.Status == ScanTaskStatus.Queued)
            .OrderBy(x => x.StageOrder)
            .ThenBy(x => x.Order)
            .FirstOrDefault();

        scan.CurrentStage = currentStage?.StageId ?? "queued";
        scan.UpdatedAt = _dateTimeProvider.UtcNow;

        scan.Events.Add(new ScanEvent
        {
            EventType = "scan.started",
            PayloadJson = null,
            CreatedAt = _dateTimeProvider.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Scan {ScanId} started successfully. {QueuedTaskCount} task(s) queued",
            scan.Id,
            queuedTaskCount);
        
        return new StartScanResult(
            ScanId: scan.Id,
            Status: scan.Status,
            QueuedTaskCount: queuedTaskCount,
            UpdatedAt: scan.UpdatedAt);
    }
}