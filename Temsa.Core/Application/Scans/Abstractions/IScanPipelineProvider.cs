using Temsa.Core.Application.Scans.Models;
using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Abstractions;

public interface IScanPipelineProvider
{
    Task<ScanPipelineDefinition> GetAsync(
        PlatformType platform,
        AnalysisType analysisType,
        CancellationToken cancellationToken = default);
}