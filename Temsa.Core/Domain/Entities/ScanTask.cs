using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Domain.Entities;

public class ScanTask
{
    public long Id { get; set; }
    public long ScanId { get; set; }
    public int Order { get; set; }

    public string TaskType { get; set; } = string.Empty;
    public string WorkerType { get; set; } = string.Empty;
    public ScanTaskStatus Status { get; set; }

    public string StageId { get; set; } = "default";
    public int StageOrder { get; set; }
    public string StageExecution { get; set; } = "sequential";
    public string RunPolicy { get; set; } = "on-success";

    public string Tool { get; set; } = string.Empty;
    public int Attempt { get; set; }
    public string? PayloadJson { get; set; }
    public string? ResultJson { get; set; }
    public string? ErrorMessage { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset?  FinishedAt { get; set; }

    public Scan Scan { get; set; } = null!;
    public ICollection<ScanEvent> Events { get; set; } = new List<ScanEvent>();
}
