using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Api.Contracts.Scans;

public record GetScanResponse(
    long ScanId,
    long ProjectId,
    long InputArtifactId,
    PlatformType Platform,
    AnalysisType AnalysisType,
    ScanStatus Status,
    string? CurrentStage,
    string? ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    IReadOnlyCollection<GetScanTaskResponse> Tasks);