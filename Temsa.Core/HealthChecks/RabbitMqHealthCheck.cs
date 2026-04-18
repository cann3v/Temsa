using Microsoft.Extensions.Diagnostics.HealthChecks;
using Temsa.Core.Infrastructure.Messaging.RabbitMq;

namespace Temsa.Core.HealthChecks;

public class RabbitMqHealthCheck(IRabbitMqConnection rabbitMqConnection): IHealthCheck
{
    private readonly IRabbitMqConnection _rabbitMqConnection = rabbitMqConnection;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var connection = await _rabbitMqConnection.GetConnectionAsync(cancellationToken);

            if (!connection.IsOpen)
            {
                return HealthCheckResult.Unhealthy("RabbitMQ connection is closed");
            }

            await using var channel = await _rabbitMqConnection.CreateChannelAsync(cancellationToken);

            return HealthCheckResult.Healthy("RabbitMQ is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ is unreachable", ex);
        }
    }
}
