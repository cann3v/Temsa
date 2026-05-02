namespace Temsa.Contracts.Artifacts;

public record ScanArtifactDescriptor(
    ArtifactKind Kind,
    string Bucket,
    string ObjectKey,
    string? FileName,
    string? ContentType,
    long? SizeBytes);