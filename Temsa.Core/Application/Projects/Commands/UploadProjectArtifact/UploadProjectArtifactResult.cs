using Temsa.Contracts.Artifacts;

namespace Temsa.Core.Application.Projects.Commands.UploadProjectArtifact;

public record UploadProjectArtifactResult(
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