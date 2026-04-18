using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Api.Contracts.Scans;

public class CreateScanRequest
{
    public long ProjectId { get; set; }
    public long InputArtifactId { get; set; }
    public PlatformType Platform { get; set; }
    public AnalysisType AnalysisType { get; set; }
}