namespace Temsa.Core.Application.Projects.Queries.GetProject;

public record GetProjectResult(
    long Id,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<long> ScanIds);