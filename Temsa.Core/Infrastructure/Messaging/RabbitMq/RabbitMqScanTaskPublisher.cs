using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Core.Application.Scans.Abstractions;
using Temsa.Contracts.Messaging.ScanTasks;

namespace Temsa.Core.Infrastructure.Messaging.RabbitMq;

public class RabbitMqScanTaskPublisher(
    IRabbitMqConnection rabbitMqConnection,
    IOptions<RabbitMqScanTasksOptions> options,
    ILogger<RabbitMqScanTaskPublisher> logger) : IScanTaskPublisher
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    
    private readonly IRabbitMqConnection _rabbitMqConnection = rabbitMqConnection;
    private readonly RabbitMqScanTasksOptions _options = options.Value;
    private readonly ILogger<RabbitMqScanTaskPublisher> _logger = logger;

    public async Task PublishAsync(string routingKey, ScanTaskDispatchMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);

        var payload = JsonSerializer.Serialize(message, JsonSerializerOptions);
        var body = Encoding.UTF8.GetBytes(payload);

        await using var channel = await _rabbitMqConnection.CreateChannelAsync(cancellationToken);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent 
        };

        await channel.BasicPublishAsync(
            exchange: _options.ScanTasksExchange,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
        
        _logger.LogInformation(
            "Published scan task {ScanTaskId} for scan {ScanId} to exchange {Exchange} with routing key {routingKey}",
            message.ScanTaskId,
            message.ScanId,
            _options.ScanTasksExchange,
            routingKey);
    }
}