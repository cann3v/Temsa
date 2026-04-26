using Microsoft.Extensions.Logging;
using Temsa.Contracts.Messaging.ScanTasks;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Execution;

public class WorkerTaskContext
{
    public required ScanTaskDispatchMessage Task { get; init; }
    public required string WorkerId { get; init; }
    public required IWorkerEventPublisher Events { get; init; }
    public required ILogger Logger { get; init; }
}