using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Common.Time;
using Temsa.Contracts.Messaging.ScanTasks;
using Temsa.Contracts.Messaging.WorkerEvents;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Messaging.RabbitMq;

public class RabbitMqWorkerEventPublisher(
    IRabbitMqConnection connection,
    IOptions<RabbitMqWorkerEventsOptions> options,
    IWorkerIdentityProvider identityProvider,
    IDateTimeProvider dateTimeProvider,
    ILogger<RabbitMqWorkerEventPublisher> logger) : IWorkerEventPublisher
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IRabbitMqConnection _connection = connection;
    private readonly RabbitMqWorkerEventsOptions _options = options.Value;
    private readonly IWorkerIdentityProvider _identityProvider = identityProvider;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<RabbitMqWorkerEventPublisher> _logger = logger;

    public Task PublishStartedAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string? message = null,
        string? log = null,
        CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            task,
            WorkerEventTypes.TaskStarted,
            workerId,
            message,
            log,
            payload: null,
            cancellationToken);
    }

    public Task PublishCompletedAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        JsonElement? payload = null,
        string? message = null,
        string? log = null,
        CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            task,
            WorkerEventTypes.TaskCompleted,
            workerId,
            message,
            log,
            payload,
            cancellationToken);
    }

    public Task PublishFailedAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string? errorMessage = null,
        string? log = null,
        JsonElement? payload = null,
        CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            task,
            WorkerEventTypes.TaskFailed,
            workerId,
            errorMessage,
            log,
            payload,
            cancellationToken);
    }

    public Task PublishProgressAsync(
        ScanTaskDispatchMessage task,
        string workerId,
        string phase,
        string? message = null,
        int? percent = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phase);
        
        var payload = JsonSerializer.SerializeToElement(
            new WorkerTaskProgressPayload(phase, percent),
            JsonSerializerOptions);

        return PublishAsync(
            task,
            WorkerEventTypes.TaskProgress,
            workerId,
            message,
            log: null,
            payload,
            cancellationToken);
    }

    public Task PublishLogAsync(ScanTaskDispatchMessage task,
        string workerId,
        string message,
        string? level = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        var payload = JsonSerializer.SerializeToElement(
            new WorkerTaskLogPayload(level),
            JsonSerializerOptions);

        return PublishAsync(
            task,
            WorkerEventTypes.TaskLog,
            workerId,
            message,
            log: message,
            payload,
            cancellationToken);
    }

    private async Task PublishAsync(
        ScanTaskDispatchMessage task,
        string eventType,
        string workerId,
        string? message,
        string? log,
        JsonElement? payload,
        CancellationToken cancellationToken)
    {
        var workerEvent = new WorkerEventMessage(
            ScanId: task.ScanId,
            ScanTaskId: task.ScanTaskId,
            EventType: eventType,
            WorkerId: string.IsNullOrWhiteSpace(workerId)
                ? _identityProvider.WorkerId
                : workerId,
            Attempt: 1, // TODO: добавить Attempt в ScanTaskDispatchMessage
            Message: message,
            Log: log,
            Payload: payload,
            OccuredAt: _dateTimeProvider.UtcNow);

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(workerEvent, JsonSerializerOptions));

        await using var channel = await _connection.CreateChannelAsync(cancellationToken);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: _options.WorkerEventsExchange,
            routingKey: eventType,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
        
        _logger.LogDebug(
            "Published worker event {EventType} for scan {ScanId}, task {ScanTaskId}",
            eventType,
            task.ScanId,
            task.ScanTaskId);
    }
}