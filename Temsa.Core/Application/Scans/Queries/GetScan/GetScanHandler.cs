using Microsoft.EntityFrameworkCore;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Scans.Queries.GetScan;

public class GetScanHandler(TemsaDbContext dbContext, ILogger<GetScanHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly ILogger<GetScanHandler> _logger = logger;

    public async Task<GetScanResult?> HandleAsync(
        GetScanQuery query,
        CancellationToken cancellationToken = default)
    {
        var scan = await _dbContext.Scans
            .AsNoTracking()
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == query.ScanId, cancellationToken);

        if (scan is null)
        {
            return null;
        }
        
        var tasks = scan.Tasks
            .OrderBy(x => x.Order)
            .Select(x => new GetScanTaskItem(
                Id: x.Id,
                TaskType: x.TaskType,
                WorkerType: x.WorkerType,
                Tool: x.Tool,
                Order: x.Order,
                Status: x.Status,
                Attempt: x.Attempt,
                ErrorMessage: x.ErrorMessage,
                CreatedAt: x.CreatedAt,
                UpdatedAt: x.UpdatedAt,
                StartedAt: x.StartedAt,
                FinishedAt: x.FinishedAt))
            .ToArray();
        
        return new GetScanResult(
            ScanId: scan.Id,
            ProjectId: scan.ProjectId,
            InputArtifactId: scan.InputArtifactId,
            Platform: scan.Platform,
            AnalysisType: scan.AnalysisType,
            Status: scan.Status,
            CurrentStage: scan.CurrentStage,
            ErrorMessage: scan.ErrorMessage,
            CreatedAt: scan.CreatedAt,
            UpdatedAt: scan.UpdatedAt,
            StartedAt: scan.StartedAt,
            FinishedAt: scan.FinishedAt,
            Tasks: tasks);
    }
}