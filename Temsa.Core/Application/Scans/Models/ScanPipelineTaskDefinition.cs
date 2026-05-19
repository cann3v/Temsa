namespace Temsa.Core.Application.Scans.Models;

public class ScanPipelineTaskDefinition
{
    public string TaskType { get; set; } = string.Empty;
    public string WorkerType { get; set; } = string.Empty;
    public string StageId { get; set; } = "default";
    public int StageOrder { get; init; }
    public string StageExecution { get; set; } = "sequential";
    public string RunPolicy { get; set; } = "on-success";
    public string Tool { get; set; } = string.Empty;
    public int Order { get; init; }
    public string? ParametersJson { get; set; }
}