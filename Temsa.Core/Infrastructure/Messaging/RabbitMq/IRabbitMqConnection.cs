using RabbitMQ.Client;

namespace Temsa.Core.Infrastructure.Messaging.RabbitMq;

public interface IRabbitMqConnection: IAsyncDisposable
{
    Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
}