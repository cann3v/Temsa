using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Execution;

public class WorkerTaskStopHandle : IWorkerTaskStopHandle
{
    private readonly TaskCompletionSource<WorkerTaskStopRequest> _stopRequested =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public bool IsStopRequested => _stopRequested.Task.IsCompleted;

    public async Task WaitForStopAsync(CancellationToken cancellationToken = default)
    {
        await _stopRequested.Task.WaitAsync(cancellationToken);
    }

    public Task RequestStopAsync(WorkerTaskStopRequest request, CancellationToken cancellationToken = default)
    {
        _stopRequested.TrySetResult(request);
        return Task.CompletedTask;
    }
}