namespace Temsa.Contracts.Messaging.WorkerControl;

public record WorkerControlMessage(
    long ScanId,
    long? ScanTaskId,
    string CommandType,
    string? Reason,
    DateTimeOffset OccuredAt);