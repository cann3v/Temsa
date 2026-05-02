using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Contracts.Messaging.ScanTasks;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.Runtime.Messaging.RabbitMq;

public class RabbitMqScanTaskConsumerHostedService(
    IRabbitMqConnection connection,
    IOptions<RabbitMqWorkerEventsOptions> options,
    IServiceScopeFactory scopeFactory,
    IWorkerIdentityProvider identityProvider,
    ILogger<RabbitMqScanTaskConsumerHostedService> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IRabbitMqConnection _connection = connection;
    private readonly RabbitMqWorkerEventsOptions _options = options.Value;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IWorkerIdentityProvider _identityProvider = identityProvider;
    private readonly ILogger<RabbitMqScanTaskConsumerHostedService> _logger = logger;
    
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Starting scan task consumer for queue {QueueName}",
            _options.QueueName);
        
        _channel = await _connection.CreateChannelAsync(stoppingToken);

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
            "Scan task consumer started for queue {QueueName}",
            _options.QueueName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessageAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        ScanTaskDispatchMessage? task = null;

        try
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());

            task = JsonSerializer.Deserialize<ScanTaskDispatchMessage>(
                body,
                JsonSerializerOptions);

            if (task is null)
            {
                throw new InvalidOperationException("Scan task message body is empty or invalid");
            }

            _logger.LogInformation(
                "Received scan task {ScanTaskId} for scan {ScanId}. TaskType={TaskType}, Tool={Tool}",
                task.ScanTaskId,
                task.ScanId,
                task.TaskType,
                task.Tool);

            using var scope = _scopeFactory.CreateScope();

            var eventPublisher = scope.ServiceProvider.GetRequiredService<IWorkerEventPublisher>();
            var handlers = scope.ServiceProvider.GetServices<IWorkerTaskHandler>();

            var handler = handlers.FirstOrDefault(x =>
                string.Equals(x.TaskType, task.TaskType, StringComparison.OrdinalIgnoreCase));

            if (handler is null)
            {
                await eventPublisher.PublishFailedAsync(
                    task,
                    _identityProvider.WorkerId,
                    errorMessage: $"No handler registered for task type '{task.TaskType}'",
                    cancellationToken: cancellationToken);

                await AckAsync(args.DeliveryTag, cancellationToken);

                _logger.LogError(
                    "No handler registered for task type {TaskType}. Task {ScanTaskId} acknowledged as failed",
                    task.TaskType,
                    task.ScanTaskId);

                return;
            }

            await eventPublisher.PublishStartedAsync(
                task,
                _identityProvider.WorkerId,
                message: $"Worker started task '{task.TaskType}' using tool '{task.Tool}'",
                cancellationToken: cancellationToken);

            await AckAsync(args.DeliveryTag, cancellationToken);

            var context = new WorkerTaskContext
            {
                Task = task,
                WorkerId = _identityProvider.WorkerId,
                Events = eventPublisher
            };

            try
            {
                var result = await handler.ExecuteAsync(context, cancellationToken);

                await eventPublisher.PublishCompletedAsync(
                    task,
                    _identityProvider.WorkerId,
                    resultJson: result.ResultJson,
                    message: result.Message ?? $"Worker completed task '{task.TaskType}'",
                    log: result.Log,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Scan task {ScanTaskId} completed successfully",
                    task.ScanTaskId);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    "Scan task {ScanTaskId} execution was cancelled due to worker shutdown",
                    task.ScanTaskId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Scan task {ScanTaskId} failed during execution",
                    task.ScanTaskId);

                await eventPublisher.PublishFailedAsync(
                    task,
                    _identityProvider.WorkerId,
                    errorMessage: ex.Message,
                    log: ex.ToString(),
                    cancellationToken: cancellationToken);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize scan task message. Message will be rejected without requeue");

            await SafeNackAsync(args.DeliveryTag, requeue: false, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed before scan task was acknowledged. Message will be requeue. " +
                "ScanId={ScanId}, ScanTaskId={ScanTaskId}",
                task?.ScanId,
                task?.ScanTaskId);

            await SafeNackAsync(args.DeliveryTag, requeue: true, cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping scan task consumer");

        if (_channel is not null)
        {
            try
            {
                await _channel.CloseAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to close scan task consumer channel");
            }

            await _channel.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }

    private async Task AckAsync(
        ulong deliveryTag,
        CancellationToken cancellationToken)
    {
        await _channel!.BasicAckAsync(
            deliveryTag: deliveryTag,
            multiple: false,
            cancellationToken: cancellationToken);
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
            _logger.LogError(
                ex,
                "Failed to nack scan task message with delivery tag '{DeliveryTag}'",
                deliveryTag);
        }
    }
}