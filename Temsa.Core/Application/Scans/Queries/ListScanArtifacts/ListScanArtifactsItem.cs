using Temsa.Contracts.Artifacts;

namespace Temsa.Core.Application.Scans.Queries.ListScanArtifacts;

public record ListScanArtifactsItem(
    long Id,
    long ScanId,
    ArtifactKind Kind,
    string Bucket,
    string ObjectKey,
    string? FileName,
    string? ContentType,
    long? SizeBytes,
    DateTimeOffset CreatedAt);