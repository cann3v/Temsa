namespace Temsa.Core.Application.Projects.Commands.CreateProject;

public record CreateProjectResult(
    long Id,
    string Name,
    DateTimeOffset CreatedAt);