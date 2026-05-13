using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Contracts.Messaging.WorkerControl;
using Temsa.Core.Application.WorkerControl.Abstractions;

namespace Temsa.Core.Infrastructure.Messaging.RabbitMq;

public class RabbitMqWorkerControlPublisher(
    IRabbitMqConnection rabbitMqConnection,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqWorkerControlPublisher> logger) : IWorkerControlPublisher
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IRabbitMqConnection _rabbitMqConnection = rabbitMqConnection;
    private readonly RabbitMqWorkerControlOptions _options = options.Value.Control;
    private readonly ILogger<RabbitMqWorkerControlPublisher> _logger = logger;

    public async Task PublishAsync(
        string routingKey,
        WorkerControlMessage message,
        CancellationToken cancellationToken = default)
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
            exchange: _options.WorkerControlExchange,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Published worker control command {CommandType} " +
            "for scan {ScanId}, task {ScanTaskId} to routing key {RoutingKey}",
            message.CommandType,
            message.ScanId,
            message.ScanTaskId,
            routingKey);
    }
}