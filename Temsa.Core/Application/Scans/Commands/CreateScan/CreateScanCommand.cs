using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Commands.CreateScan;

public record CreateScanCommand(
    long ProjectId,
    long InputArtifactId,
    PlatformType Platform,
    AnalysisType AnalysisType);