namespace Temsa.Core.Configuration;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}