using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Execution;

public record RunningWorkerTask(
    long ScanId,
    long ScanTaskId,
    string TaskType,
    string WorkerId,
    IWorkerTaskStopHandle StopHandle,
    DateTimeOffset StartedAt);