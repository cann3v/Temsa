using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.Runtime.Abstractions;

public interface IRunningWorkerTaskRegistry
{
    void Register(RunningWorkerTask task);

    void Unregister(long scanTaskId);
    
    bool TryGetByTaskId(
        long scanTaskId,
        out RunningWorkerTask task);
    
    IReadOnlyCollection<RunningWorkerTask> GetByScanId(long scanId);
}