using Microsoft.EntityFrameworkCore;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Scans.Queries.ListScanTaskEvents;

public class ListScanTaskEventsHandler(TemsaDbContext dbContext)
{
    private readonly TemsaDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<ListScanTaskEventsItem>?> HandleAsync(
        ListScanTaskEventsQuery query,
        CancellationToken cancellationToken = default)
    {
        var taskExists = await _dbContext.ScanTasks
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == query.ScanTaskId && x.ScanId == query.ScanId,
                cancellationToken);

        if (!taskExists)
        {
            return null;
        }
        
        return await _dbContext.ScanEvents
            .AsNoTracking()
            .Where(x =>
                x.ScanId == query.ScanId &&
                x.ScanTaskId == query.ScanTaskId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new ListScanTaskEventsItem(
                x.Id,
                x.ScanId,
                x.ScanTaskId,
                x.EventType,
                x.PayloadJson,
                x.CreatedAt))
            .ToArrayAsync(cancellationToken);
    }
}