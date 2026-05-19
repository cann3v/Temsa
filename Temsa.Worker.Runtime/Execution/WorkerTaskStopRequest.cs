namespace Temsa.Worker.Runtime.Execution;

public record WorkerTaskStopRequest(
    long ScanId,
    long? ScanTaskId,
    string Reason,
    DateTimeOffset RequestedAt);