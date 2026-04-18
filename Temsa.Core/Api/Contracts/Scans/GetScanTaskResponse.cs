using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Api.Contracts.Scans;

public record GetScanTaskResponse(
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