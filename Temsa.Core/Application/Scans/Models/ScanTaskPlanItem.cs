namespace Temsa.Core.Application.Scans.Models;

public record ScanTaskPlanItem(
    string TaskType,
    string WorkerType,
    string Tool,
    int Order,
    string? PayloadJson);