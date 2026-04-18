using System.Runtime.InteropServices.JavaScript;

namespace Temsa.Core.Api.Contracts.Projects;

public record ListProjectsItemResponse(
    long Id,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);