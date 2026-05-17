namespace Temsa.Core.Application.Scans.Queries.GetScanArtifactContent;

public record GetScanArtifactContentResult(
    Stream Content,
    string FileName,
    string ContentType);