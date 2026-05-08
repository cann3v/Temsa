using Temsa.Contracts.Artifacts;

namespace Temsa.Core.Api.Contracts.Projects.Artifacts;

public record UploadProjectArtifactResponse(
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