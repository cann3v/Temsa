namespace Temsa.Core.Configuration;

public class RabbitMqMessagingOptions
{
    public const string SectionName = "RabbitMqMessaging";
    
    public string ScanTasksExchange { get; init; } = string.Empty;
    public string WorkerEventsExchange { get; init; } = string.Empty;
    
    public string StaticAnalysisQueue { get; init; } = string.Empty;
    public string AndroidDynamicAnalysisQueue { get; init; } = string.Empty;
    public string IosDynamicAnalysisQueue { get; init; } = string.Empty;
    
    public RabbitMqWorkerEventsConsumerOptions Consumer { get; init; } = new();
}