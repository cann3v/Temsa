using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Execution;

public record WorkerTaskExecutionControl(
    IWorkerTaskEventSink Events,
    IWorkerTaskStopHandle StopHandle);