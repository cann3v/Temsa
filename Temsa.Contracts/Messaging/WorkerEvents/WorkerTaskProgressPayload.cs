namespace Temsa.Contracts.Messaging.WorkerEvents;

public record WorkerTaskProgressPayload(
    string Phase,
    int? Percent);