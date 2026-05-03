namespace Temsa.Core.Domain.Entities;

public class ScanEvent
{
    public long Id { get; set; }
    public long ScanId { get; set; }
    public long? ScanTaskId { get; set; }

    public string EventType { get; set; } = string.Empty;
    public string? PayloadJson { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }

    public Scan Scan { get; set; } = null!;
    public ScanTask? ScanTask { get; set; }
}