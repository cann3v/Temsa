using Temsa.Contracts.Messaging.ScanTasks;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Execution;

public class WorkerTaskEventSink(
    ScanTaskDispatchMessage task,
    string workerId,
    IWorkerEventPublisher eventPublisher) : IWorkerTaskEventSink
{
    private readonly ScanTaskDispatchMessage _task = task;
    private readonly string _workerId = workerId;
    private readonly IWorkerEventPublisher _eventPublisher = eventPublisher;

    public Task ReportProgressAsync(
        string phase,
        string? message = null,
        int? percent = null,
        CancellationToken cancellationToken = default)
    {
        return _eventPublisher.PublishProgressAsync(
            _task,
            _workerId,
            phase,
            message,
            percent,
            cancellationToken);
    }

    public Task ReportLogAsync(
        string message,
        string? level = null,
        CancellationToken cancellationToken = default)
    {
        return _eventPublisher.PublishLogAsync(
            _task,
            _workerId,
            message,
            level,
            cancellationToken);
    }
}