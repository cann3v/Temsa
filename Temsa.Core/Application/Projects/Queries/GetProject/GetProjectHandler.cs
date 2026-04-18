using Microsoft.EntityFrameworkCore;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Projects.Queries.GetProject;

public class GetProjectHandler(TemsaDbContext dbContext, ILogger<GetProjectHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly ILogger<GetProjectHandler> _logger = logger;

    public async Task<GetProjectResult?> HandleAsync(
        GetProjectQuery query,
     
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
            .Select(x => new GetProjectResult(
                x.Id,
                x.Name,
                x.CreatedAt,
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}