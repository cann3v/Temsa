using Microsoft.EntityFrameworkCore;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Scans.Queries.ListScanArtifacts;

public class ListScanArtifactsHandler(TemsaDbContext dbContext)
{
    private readonly TemsaDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<ListScanArtifactsItem>?> HandleAsync(
        ListScanArtifactsQuery query,
        CancellationToken cancellationToken = default)
    {
        var scanExists = await _dbContext.Scans
            .AsNoTracking()
            .AnyAsync(x => x.Id == query.ScanId, cancellationToken);

        if (!scanExists)
        {
            return null;
        }
        
        return await _dbContext.ScanArtifacts
            .AsNoTracking()
            .Where(x => x.Id == query.ScanId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ListScanArtifactsItem(
                x.Id,
                x.ScanId,
                x.Kind,
                x.Bucket,
                x.ObjectKey,
                x.FileName,
                x.ContentType,
                x.SizeBytes,
                x.CreatedAt))
            .ToArrayAsync(cancellationToken);
    }
}