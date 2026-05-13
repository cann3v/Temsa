using Temsa.Contracts.Artifacts;

namespace Temsa.Core.Api.Contracts.Scans.Artifacts;

public record GetScanArtifactResponse(
    long Id,
    long ScanId,
    ArtifactKind Kind,
    string Bucket,
    string ObjectKey,
    string? FileName,
    string? ContentType,
    long? SizeBytes,
    DateTimeOffset CreatedAt);