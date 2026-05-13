using Temsa.Contracts.Artifacts;

namespace Temsa.Core.Application.Projects.Queries.ListProjectArtifacts;

public record ListProjectArtifactsItem(
    long Id,
    long ProjectId,
    ProjectArtifactType Type,
    ArtifactKind Kind,
    string Bucket,
    string ObjectKey,
    string? FileName,
    string? ContentType,
    long? SizeBytes,
    DateTimeOffset CreatedAt);