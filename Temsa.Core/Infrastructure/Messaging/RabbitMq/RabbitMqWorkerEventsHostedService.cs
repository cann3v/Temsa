using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Temsa.Contracts.Messaging.WorkerEvents;
using Temsa.Core.Application.WorkerEvents.Commands.HandleWorkerEvent;
using Temsa.Core.Configuration;

namespace Temsa.Core.Infrastructure.Messaging.RabbitMq;

public class RabbitMqWorkerEventsHostedService(
    IRabbitMqConnection rabbitMqConnection,
    IOptions<RabbitMqMessagingOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqWorkerEventsHostedService> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    
    private readonly IRabbitMqConnection _rabbitMqConnection = rabbitMqConnection;
    private readonly RabbitMqWorkerEventsConsumerOptions _options = options.Value.Consumer;
    private readonly IServiceScopeFactory _scopeFactory =  scopeFactory;
    private readonly ILogger<RabbitMqWorkerEventsHostedService> _logger = logger;

    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Starting RabbitMq worker events consumer for queue {QueueName}",
            _options.QueueName);

        _channel = await _rabbitMqConnection.CreateChannelAsync(stoppingToken);

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: _options.PrefetchCount,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            await HandleMessageAsync(args, stoppingToken);
        };
        
        await _channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
        
        _logger.LogInformation(
            "RabbitMQ worker events consumer started for queue {QueueName}",
            _options.QueueName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessageAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        WorkerEventMessage? message = null;

        try
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());

            message = JsonSerializer.Deserialize<WorkerEventMessage>(
                body,
                JsonSerializerOptions);

            if (message is null)
            {
                throw new InvalidOperationException("Worker event message body is empty or invalid");
            }

            _logger.LogDebug(
                "Received worker event {EventType} for scan {ScanId}, task {ScanTaskId}",
                message.EventType,
                message.ScanId,
                message.ScanTaskId);

            using var scope = _scopeFactory.CreateScope();

            var handler = scope.ServiceProvider
                .GetRequiredService<HandleWorkerEventHandler>();

            await handler.HandleAsync(
                new HandleWorkerEventCommand(message),
                cancellationToken);

            await _channel!.BasicAckAsync(
                deliveryTag: args.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);

            _logger.LogDebug(
                "Worker event {EventType} for scan {ScanId}, task {ScanTaskId} acknowledged",
                message.EventType,
                message.ScanId,
                message.ScanTaskId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex,
                "Failed to handle worker event. Message will be rejected without requeue. " +
                "ScanId={ScanId}, ScanTaskId={ScanTaskId}, EventType={EventType}",
                message?.ScanId,
                message?.ScanTaskId,
                message?.EventType);

            await SafeRejectAsync(args.DeliveryTag, requeue: false, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while handling worker event. Message will be requeued. " +
                "ScanId={ScanId}, ScanTaskId={ScanTaskId}, EventType={EventType}",
                message?.ScanId,
                message?.ScanTaskId,
                message?.EventType);

            await SafeRejectAsync(args.DeliveryTag, requeue: true, cancellationToken);
        }
    }

    private async Task SafeRejectAsync(
        ulong deliveryTag,
        bool requeue,
        CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            return;
        }

        try
        {
            await _channel.BasicNackAsync(
                deliveryTag: deliveryTag,
                multiple: false,
                requeue: requeue,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to nack worker event message with delivery tag {DeliveryTag}",
                deliveryTag);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ worker event consumer");

        if (_channel is not null)
        {
            try
            {
                await _channel.CloseAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to close RabbitMQ worker event consumer");
            }

            await _channel.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}