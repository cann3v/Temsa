using Temsa.Contracts.Messaging.WorkerControl;

namespace Temsa.Core.Application.WorkerControl.Abstractions;

public interface IWorkerControlPublisher
{
    Task PublishAsync(
        string routingKey,
        WorkerControlMessage message,
        CancellationToken cancellationToken = default);
}