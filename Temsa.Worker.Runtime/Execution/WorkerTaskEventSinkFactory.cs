using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Execution;

public class WorkerTaskEventSinkFactory(
    IWorkerEventPublisher eventPublisher) : IWorkerTaskEventSinkFactory
{
    private readonly IWorkerEventPublisher _eventPublisher = eventPublisher;

    public IWorkerTaskEventSink Create(WorkerTaskContext context)
    {
        return new WorkerTaskEventSink(
            context.Task,
            context.WorkerId,
            _eventPublisher);
    }
}