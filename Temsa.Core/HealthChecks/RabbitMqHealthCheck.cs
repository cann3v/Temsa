using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Temsa.Core.Configuration;
using Temsa.Core.Infrastructure.Messaging.RabbitMq;

namespace Temsa.Core.HealthChecks;

public class RabbitMqHealthCheck(
    IRabbitMqConnection rabbitMqConnection,
    IOptions<RabbitMqMessagingOptions> messagingOptions,
    ILogger<RabbitMqHealthCheck> logger): IHealthCheck
{
    private readonly IRabbitMqConnection _rabbitMqConnection = rabbitMqConnection;
    private readonly RabbitMqMessagingOptions _messagingOptions = messagingOptions.Value;
    private readonly ILogger<RabbitMqHealthCheck> _logger = logger;

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

            await channel.ExchangeDeclarePassiveAsync(
                _messagingOptions.ScanTasksExchange,
                cancellationToken);

            await channel.QueueDeclarePassiveAsync(
                _messagingOptions.StaticAnalysisQueue,
                cancellationToken);
            
            await channel.QueueDeclarePassiveAsync(
                _messagingOptions.AndroidDynamicAnalysisQueue,
                cancellationToken);

            await channel.QueueDeclarePassiveAsync(
                _messagingOptions.IosDynamicAnalysisQueue,
                cancellationToken);

            return HealthCheckResult.Healthy("RabbitMQ is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ is unreachable or topology is missing", ex);
        }
    }
}
