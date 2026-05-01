namespace Temsa.Common.Configuration;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    
    public RabbitMqConnectionOptions Connection { get; init; } = new();
    public RabbitMqScanTasksOptions Producer { get; init; } = new();
    public RabbitMqWorkerEventsOptions Consumer { get; init; } = new();
}

public class RabbitMqWorkerOptions
{
    public const string SectionName = "RabbitMq";
    
    public RabbitMqConnectionOptions Connection { get; init; } = new();
    public RabbitMqWorkerEventsOptions Messaging { get; init; } = new RabbitMqWorkerEventsOptions{PrefetchCount = 1};
}

public class RabbitMqConnectionOptions
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 5672;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class RabbitMqScanTasksOptions
{
    /// <summary>
    /// Обменник для задач сканирования
    /// </summary>
    public string ScanTasksExchange { get; init; } = string.Empty;
    /// <summary>
    /// Очередь для SAST задач
    /// </summary>
    
    public string StaticAnalysisQueue { get; init; } = string.Empty;
    /// <summary>
    /// Очередь для DAST Android задач
    /// </summary>
    public string AndroidDynamicAnalysisQueue { get; init; } = string.Empty;
    /// <summary>
    /// Очередь для DAST iOS задач
    /// </summary>
    public string IosDynamicAnalysisQueue { get; init; } = string.Empty;
}

public class RabbitMqWorkerEventsOptions
{
    /// <summary>
    /// Обменник событий воркера
    /// </summary>
    public string WorkerEventsExchange { get; init; } = string.Empty;
    /// <summary>
    /// Очередь задач для воркера
    /// </summary>
    public string QueueName { get; init; } = string.Empty;
    public ushort PrefetchCount { get; init; } = 10;
}