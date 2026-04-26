using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Core.Configuration;
using Temsa.Core.Infrastructure.Messaging.RabbitMq;

namespace Temsa.Core.HealthChecks;

public class RabbitMqHealthCheck(
    IRabbitMqConnection rabbitMqConnection,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqHealthCheck> logger): IHealthCheck
{
    private readonly IRabbitMqConnection _rabbitMqConnection = rabbitMqConnection;
    private readonly RabbitMqScanTasksOptions _producerOptions = options.Value.Producer;
    private readonly RabbitMqWorkerEventsOptions _consumerOptions = options.Value.Consumer;
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
                _producerOptions.ScanTasksExchange,
                cancellationToken);

            await channel.ExchangeDeclarePassiveAsync(
                _consumerOptions.WorkerEventsExchange,
                cancellationToken);

            await channel.QueueDeclarePassiveAsync(
                _producerOptions.StaticAnalysisQueue,
                cancellationToken);
            
            await channel.QueueDeclarePassiveAsync(
                _producerOptions.AndroidDynamicAnalysisQueue,
                cancellationToken);

            await channel.QueueDeclarePassiveAsync(
                _producerOptions.IosDynamicAnalysisQueue,
                cancellationToken);

            await channel.QueueDeclarePassiveAsync(
                _consumerOptions.QueueName,
                cancellationToken);

            return HealthCheckResult.Healthy("RabbitMQ is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ is unreachable or topology is missing", ex);
        }
    }
}
