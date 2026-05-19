using System.Collections.Concurrent;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Execution;

public class RunningWorkerTaskRegistry : IRunningWorkerTaskRegistry
{
    private readonly ConcurrentDictionary<long, RunningWorkerTask> _tasksByTaskid = new();

    public void Register(RunningWorkerTask task)
    {
        _tasksByTaskid[task.ScanTaskId] = task;
    }

    public void Unregister(long scanTaskId)
    {
        _tasksByTaskid.TryRemove(scanTaskId, out _);
    }

    public bool TryGetByTaskId(long scanTaskId, out RunningWorkerTask task)
    {
        return _tasksByTaskid.TryGetValue(scanTaskId, out task!);
    }

    public IReadOnlyCollection<RunningWorkerTask> GetByScanId(long scanId)
    {
        return _tasksByTaskid.Values
            .Where(x => x.ScanId == scanId)
            .ToArray();
    }
}