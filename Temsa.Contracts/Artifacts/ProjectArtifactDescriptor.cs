namespace Temsa.Contracts.Artifacts;

public record ProjectArtifactDescriptor(
    long Id,
    ProjectArtifactType Type,
    ArtifactKind Kind,
    string Bucket,
    string ObjectKey,
    string? FileName,
    string? ContentType,
    long? SizeBytes);