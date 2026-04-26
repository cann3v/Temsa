using Microsoft.EntityFrameworkCore;
using Temsa.Common.Time;
using Temsa.Core.Application.Scans.Abstractions;
using Temsa.Core.Application.Scans.Services;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Domain.Enums;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Scans.Commands.CreateScan;

public class CreateScanHandler(
    TemsaDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ScanStatusCalculator  scanStatusCalculator,
    IScanPipelineProvider scanPipelineProvider,
    ILogger<CreateScanHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ScanStatusCalculator _scanStatusCalculator = scanStatusCalculator;
    private readonly IScanPipelineProvider _scanPipelineProvider = scanPipelineProvider;
    private readonly ILogger<CreateScanHandler> _logger = logger;

    public async Task<CreateScanResult> HandleAsync(
        CreateScanCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Creating scan for project {ProjectId}, artifact {ArtifactId}, platform {Platform}, analysis {AnalysisType}",
            command.ProjectId,
            command.InputArtifactId,
            command.Platform,
            command.AnalysisType);
        
        var projectExists = await _dbContext.Projects.AnyAsync(
            x => x.Id == command.ProjectId, cancellationToken);

        if (!projectExists)
        {
            throw new InvalidOperationException($"Project with id {command.ProjectId} was not found");
        }

        var now = _dateTimeProvider.UtcNow;

        var pipeline = await _scanPipelineProvider.GetAsync(
            command.Platform,
            command.AnalysisType,
            cancellationToken);
        
        _logger.LogDebug(
            "Loaded pipeline with {TaskCount} tasks for platform {Platform} and analysis {AnalysisType}",
            pipeline.Tasks.Count,
            command.Platform,
            command.AnalysisType);

        var scan = new Scan
        {
            ProjectId = command.ProjectId,
            InputArtifactId = command.InputArtifactId,
            Platform = command.Platform,
            AnalysisType = command.AnalysisType,
            Status = ScanStatus.Pending,
            CurrentStage = "created",
            CreatedAt = now,
            UpdatedAt = now
        };

        foreach (var taskDefinition in pipeline.Tasks.OrderBy(x => x.Order))
        {
            scan.Tasks.Add(new ScanTask
            {
                TaskType = taskDefinition.TaskType,
                WorkerType = taskDefinition.WorkerType,
                Tool = taskDefinition.Tool,
                Order = taskDefinition.Order,
                Status = ScanTaskStatus.Pending,
                Attempt = 1,
                PayloadJson = taskDefinition.ParametersJson,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        scan.Status = _scanStatusCalculator.Calculate(scan.Tasks.Select(x => x.Status).ToArray());
        
        scan.Events.Add(new ScanEvent
        {
            EventType = "scan.created",
            PayloadJson = null,
            CreatedAt = now
        });

        _dbContext.Scans.Add(scan);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Created new scan, id = {scanId}, project id = {projectId}, task count = {taskCount}",
            scan.Id, scan.ProjectId, scan.Tasks.Count);

        return new CreateScanResult(
            ScanId: scan.Id,
            Status: scan.Status,
            TaskCount: scan.Tasks.Count,
            CreatedAt: scan.CreatedAt);
    }
}