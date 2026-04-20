using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Commands.StartScan;

public record StartScanResult(
    long ScanId,
    ScanStatus Status,
    int QueuedTaskCount,
    DateTimeOffset UpdatedAt);