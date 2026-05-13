namespace Temsa.Contracts.Messaging.WorkerControl;

public record WorkerControlMessage(
    long ScanId,
    long ScanTaskId,
    string CommandType,
    DateTimeOffset OccuredAt);