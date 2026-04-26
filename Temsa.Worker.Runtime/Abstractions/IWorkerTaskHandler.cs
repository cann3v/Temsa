using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.Runtime.Abstractions;

public interface IWorkerTaskHandler
{
    string TaskType { get; }
    
    Task ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default);
}