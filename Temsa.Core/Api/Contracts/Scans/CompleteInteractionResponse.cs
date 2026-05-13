namespace Temsa.Core.Api.Contracts.Scans;

public record CompleteInteractionResponse(
    long ScanId,
    long ScanTaskId,
    string CommandType,
    DateTimeOffset OccuredAt);