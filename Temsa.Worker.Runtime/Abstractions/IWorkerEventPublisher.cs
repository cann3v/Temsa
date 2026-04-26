using Temsa.Contracts.Messaging.ScanTasks;

namespace Temsa.Worker.Runtime.Abstractions;

public interface IWorkerEventPublisher
{
    Task PublishStartedAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string? message = null,
        string? log = null,
        CancellationToken cancellationToken = default);
    
    Task PublishCompletedAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string? resultJson = null,
        string? message = null,
        string? log = null,
        CancellationToken cancellationToken = default);
    
    Task PublishFailedAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string? errorMessage = null,
        string? log = null,
        string? resultJson = null,
        CancellationToken cancellationToken = default);
}