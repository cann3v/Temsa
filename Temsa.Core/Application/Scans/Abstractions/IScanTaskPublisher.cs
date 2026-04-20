using Temsa.Core.Application.Scans.Models;

namespace Temsa.Core.Application.Scans.Abstractions;

public interface IScanTaskPublisher
{
    Task PublishAsync(
        string routingKey,
        ScanTaskDispatchMessage message,
        CancellationToken cancellationToken = default);
}