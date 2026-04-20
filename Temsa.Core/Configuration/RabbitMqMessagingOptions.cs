namespace Temsa.Core.Configuration;

public class RabbitMqMessagingOptions
{
    public const string SectionName = "RabbitMqMessaging";
    
    public string ScanTasksExchange { get; init; } = string.Empty;
}