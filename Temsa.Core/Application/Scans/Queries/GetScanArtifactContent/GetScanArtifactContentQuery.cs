namespace Temsa.Core.Application.Scans.Queries.GetScanArtifactContent;

public record GetScanArtifactContentQuery(
    long ScanId,
    long ArtifactId);