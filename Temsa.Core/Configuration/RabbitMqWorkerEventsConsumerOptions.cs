namespace Temsa.Core.Configuration;

public class RabbitMqWorkerEventsConsumerOptions
{
    public string QueueName { get; init; } = string.Empty;
    public ushort PrefetchCount { get; init; } = 10;
}