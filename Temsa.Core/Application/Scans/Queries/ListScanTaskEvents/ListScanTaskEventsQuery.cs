namespace Temsa.Core.Application.Scans.Queries.ListScanTaskEvents;

public record ListScanTaskEventsQuery(
    long ScanId,
    long ScanTaskId);