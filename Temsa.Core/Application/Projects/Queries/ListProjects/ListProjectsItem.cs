namespace Temsa.Core.Application.Projects.Queries.ListProjects;

public record ListProjectsItem(
    long Id,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);