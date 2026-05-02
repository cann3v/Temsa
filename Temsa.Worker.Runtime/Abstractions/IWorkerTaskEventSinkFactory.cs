using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.Runtime.Abstractions;

public interface IWorkerTaskEventSinkFactory
{
    IWorkerTaskEventSink Create(WorkerTaskContext context);
}