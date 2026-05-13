using Microsoft.EntityFrameworkCore;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Projects.Queries.ListProjectArtifacts;

public class ListProjectArtifactsHandler(TemsaDbContext dbContext)
{
    private readonly TemsaDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<ListProjectArtifactsItem>?> HandleAsync(
        ListProjectArtifactsQuery query,
        CancellationToken cancellationToken = default)
    {
        var projectExists = await _dbContext.Projects
            .AsNoTracking()
            .AnyAsync(x => x.Id == query.ProjectId, cancellationToken);

        if (!projectExists)
        {
            return null;
        }

        return await _dbContext.ProjectArtifacts
            .AsNoTracking()
            .Where(x => x.ProjectId == query.ProjectId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ListProjectArtifactsItem(
                x.Id,
                x.ProjectId,
                x.Type,
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