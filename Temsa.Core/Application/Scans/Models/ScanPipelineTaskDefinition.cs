namespace Temsa.Core.Application.Scans.Models;

public class ScanPipelineTaskDefinition
{
    public string TaskType { get; set; } = string.Empty;
    public string WorkerType { get; set; } = string.Empty;
    public string Tool { get; set; } = string.Empty;
    public int Order { get; init; }
    public string? ParametersJson { get; set; }
}