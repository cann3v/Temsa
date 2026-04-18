using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Models;

public class ScanPipelineDefinition
{
    public PlatformType Platform { get; set; }
    public AnalysisType AnalysisType { get; set; }
    public IReadOnlyCollection<ScanPipelineTaskDefinition> Tasks { get; init; } = [];
}