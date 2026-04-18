using Microsoft.EntityFrameworkCore;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Projects.Queries.ListProjects;

public class ListProjectsHandler(TemsaDbContext dbContext)
{
    private readonly TemsaDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<ListProjectsItem>> HandleAsync(
        ListProjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .OrderByDescending(x => x.UpdatedAt)
            .Select(x => new ListProjectsItem(
                x.Id,
                x.Name,
                x.CreatedAt,
                x.UpdatedAt))
            .ToArrayAsync(cancellationToken);
    }
}