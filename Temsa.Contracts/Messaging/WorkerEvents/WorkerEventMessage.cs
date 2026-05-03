using System.Text.Json;

namespace Temsa.Contracts.Messaging.WorkerEvents;

public record WorkerEventMessage(
    long ScanId,
    long ScanTaskId,
    string EventType,
    string WorkerId,
    int Attempt,
    string? Message,
    string? Log,
    JsonElement? Payload,
    DateTimeOffset OccuredAt);