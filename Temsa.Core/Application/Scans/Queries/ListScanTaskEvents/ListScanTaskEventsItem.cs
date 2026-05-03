namespace Temsa.Core.Application.Scans.Queries.ListScanTaskEvents;

public record ListScanTaskEventsItem(
    long Id,
    long ScanId,
    long? ScanTaskId,
    string EventType,
    string? PayloadJson,
    DateTimeOffset CreatedAt);