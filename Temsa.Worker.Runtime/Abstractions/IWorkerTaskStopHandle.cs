using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.Runtime.Abstractions;

public interface IWorkerTaskStopHandle
{
    bool IsStopRequested { get; }

    Task WaitForStopAsync(CancellationToken cancellationToken = default);
    
    Task RequestStopAsync(WorkerTaskStopRequest request, CancellationToken cancellationToken = default);
}