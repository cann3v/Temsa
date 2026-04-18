using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Api.Contracts.Scans;

public record CreateScanResponse(
    long ScanId,
    ScanStatus Status,
    int TaskCount,
    DateTimeOffset CreatedAt);