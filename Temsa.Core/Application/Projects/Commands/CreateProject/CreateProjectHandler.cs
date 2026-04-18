using Microsoft.EntityFrameworkCore;
using Temsa.Core.Application.Abstractions.Time;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Projects.Commands.CreateProject;

public class CreateProjectHandler(
    TemsaDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateProjectHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<CreateProjectHandler> _logger = logger;

    public async Task<CreateProjectResult> HandleAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = command.Name.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new InvalidOperationException("Project name cannot be empty");
        }

        var exists = await _dbContext.Projects.AnyAsync(
            x => x.Name == normalizedName, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"Project with name '{normalizedName}' already exists");
        }

        var now = _dateTimeProvider.UtcNow;

        var project = new Project
        {
            Name = normalizedName,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Created project: id = {ProjectId}, name = {ProjectName}", project.Id, project.Name);

        return new CreateProjectResult(
            project.Id,
            project.Name,
            project.CreatedAt);
    }
}