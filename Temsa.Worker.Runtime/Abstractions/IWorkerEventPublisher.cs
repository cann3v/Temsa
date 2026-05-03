using System.Text.Json;
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
        JsonElement? payload = null,
        string? message = null,
        string? log = null,
        CancellationToken cancellationToken = default);
    
    Task PublishFailedAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string? errorMessage = null,
        string? log = null,
        JsonElement? payload = null,
        CancellationToken cancellationToken = default);
    
    Task PublishProgressAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string phase,
        string? message = null,
        int? percent = null,
        CancellationToken cancellationToken = default);
    
    Task PublishLogAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string message,
        string? level = null,
        CancellationToken cancellationToken = default);
}