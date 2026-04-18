namespace Temsa.Core.Api.Contracts.Projects;

public record CreateProjectResponse(
    long Id,
    string Name,
    DateTimeOffset CreatedAt);