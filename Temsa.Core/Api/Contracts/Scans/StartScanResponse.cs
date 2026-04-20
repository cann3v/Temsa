using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Api.Contracts.Scans;

public record StartScanResponse(
    long ScanId,
    ScanStatus Status,
    int QueuedTaskCount,
    DateTimeOffset UpdatedAt);