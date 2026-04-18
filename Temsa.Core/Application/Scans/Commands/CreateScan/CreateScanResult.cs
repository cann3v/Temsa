using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Commands.CreateScan;

public record CreateScanResult(
    long ScanId,
    ScanStatus Status,
    int TaskCount,
    DateTimeOffset CreatedAt);