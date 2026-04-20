namespace Temsa.Core.Api.Contracts.Projects;

public record GetProjectResponse(
    long Id,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<long> ScanIds);