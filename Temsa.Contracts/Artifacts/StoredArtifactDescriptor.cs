namespace Temsa.Contracts.Artifacts;

public record StoredArtifactDescriptor(
    string Bucket,
    string ObjectKey,
    string? FileName,
    string? ContentType,
    long? SizeBytes);