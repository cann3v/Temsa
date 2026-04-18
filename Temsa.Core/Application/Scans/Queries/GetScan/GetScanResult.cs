using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Queries.GetScan;

public record GetScanResult(
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
    IReadOnlyCollection<GetScanTaskItem> Tasks);
    
public record GetScanTaskItem(
    long Id,
    string TaskType,
    string WorkerType,
    string Tool,
    int Order,
    ScanTaskStatus Status,
    int Attempt,
    string? ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt);