namespace Temsa.Contracts.Messaging.WorkerEvents;

public record WorkerEventMessage(
    long ScanId,
    long ScanTaskId,
    string EventType,
    string WorkerId,
    int Attempt,
    string? Message,
    string? Log,
    string? ResultJson,
    DateTimeOffset OccuredAt);