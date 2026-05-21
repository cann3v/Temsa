using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Contracts.Messaging.WorkerControl;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.Runtime.Messaging.RabbitMq;

public class RabbitMqWorkerControlConsumerHostedService(
    IRabbitMqConnection connection,
    IOptions<RabbitMqWorkerControlOptions> options,
    IRunningWorkerTaskRegistry runningTaskRegistry,
    ILogger<RabbitMqWorkerControlConsumerHostedService> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IRabbitMqConnection _connection = connection;
    private readonly RabbitMqWorkerControlOptions _options = options.Value;
    private readonly IRunningWorkerTaskRegistry _runningTaskRegistry = runningTaskRegistry;
    private readonly ILogger<RabbitMqWorkerControlConsumerHostedService> _logger = logger;

    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _connection.CreateChannelAsync(stoppingToken);
        
        // TODO перенести в healthcheck
        await _channel.ExchangeDeclareAsync(
            exchange: _options.WorkerControlExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);
        
        await _channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);
        
        await _channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.WorkerControlExchange,
            routingKey: _options.RoutingKey,
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
            "Worker control consumer started. Queue={QueueName}, RoutingKey={RoutingKey}",
            _options.QueueName,
            _options.RoutingKey);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessageAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        WorkerControlMessage? message = null;

        try
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());

            message = JsonSerializer.Deserialize<WorkerControlMessage>(
                body,
                JsonSerializerOptions);

            if (message is null)
            {
                throw new InvalidOperationException("Worker control message body is empty or invalid");
            }

            await RequestStopAsync(message, cancellationToken);

            await _channel!.BasicAckAsync(
                deliveryTag: args.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid worker control message. Rejecting without requeue");

            await SafeNackAsync(
                args.DeliveryTag,
                requeue: false,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to handle worker control command. " +
                "ScanId={ScanId}, ScanTaskId={ScanTaskId}, CommandType={CommandType}",
                message?.ScanId,
                message?.ScanTaskId,
                message?.CommandType);

            await SafeNackAsync(
                args.DeliveryTag,
                requeue: true,
                cancellationToken);
        }
    }

    private async Task RequestStopAsync(
        WorkerControlMessage message,
        CancellationToken cancellationToken)
    {
        if (!IsStopCommand(message.CommandType))
        {
            _logger.LogWarning(
                "Unsupported worker control command {CommandType}",
                message.CommandType);

            return;
        }
        
        var reason = message.Reason ?? message.CommandType;

        if (message.ScanTaskId is not null)
        {
            if (!_runningTaskRegistry.TryGetByTaskId(message.ScanTaskId.Value, out var task))
            {
                _logger.LogWarning(
                    "Stop command received, but running task was not found. ScanId={ScanId}, ScanTaskId={ScanTaskId}",
                    message.ScanId,
                    message.ScanTaskId);

                return;
            }
            
            await task.StopHandle.RequestStopAsync(
                new WorkerTaskStopRequest(
                    ScanId: message.ScanId,
                    ScanTaskId: message.ScanTaskId,
                    Reason: reason,
                    RequestedAt: message.OccuredAt),
                cancellationToken);
            
            _logger.LogInformation(
                "Stop requested for scan task {ScanTaskId}",
                message.ScanTaskId);

            return;
        }
        
        var runningTasks = _runningTaskRegistry.GetByScanId(message.ScanId);
        
        foreach (var task in runningTasks)
        {
            await task.StopHandle.RequestStopAsync(
                new WorkerTaskStopRequest(
                    ScanId: message.ScanId,
                    ScanTaskId: task.ScanTaskId,
                    Reason: reason,
                    RequestedAt: message.OccuredAt),
                cancellationToken);
        }
        
        _logger.LogInformation(
            "Stop requested for {TasksCount} running tasks in scan {ScanId}",
            runningTasks.Count,
            message.ScanId);
    }
    
    private static bool IsStopCommand(string commandType)
    {
        return string.Equals(commandType, WorkerControlCommandTypes.InteractionCompleted, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(commandType, WorkerControlCommandTypes.ScanStop, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(commandType, WorkerControlCommandTypes.ScanTaskStop, StringComparison.OrdinalIgnoreCase);
    }

    private async Task SafeNackAsync(
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
            _logger.LogError(ex, "Failed to nack worker control message");
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}