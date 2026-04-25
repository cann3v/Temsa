using Temsa.Contracts.Messaging.ScanTasks;

namespace Temsa.Core.Application.Scans.Abstractions;

public interface IScanTaskPublisher
{
    Task PublishAsync(
        string routingKey,
        ScanTaskDispatchMessage message,
        CancellationToken cancellationToken = default);
}