using System.Text.Json;

namespace Temsa.Core.Api.Contracts.Scans.ScanTasks;

public record GetScanTaskEventResponse(
    long Id,
    long ScanId,
    long? ScanTaskId,
    string EventType,
    JsonElement? Payload,
    DateTimeOffset CreatedAt);