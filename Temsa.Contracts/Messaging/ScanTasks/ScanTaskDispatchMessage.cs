namespace Temsa.Contracts.Messaging.ScanTasks;

public record ScanTaskDispatchMessage(
    long ScanTaskId,
    long ScanId,
    long InputArtifactId,
    string Platform,
    string TaskType,
    string Tool,
    string? ParametersJson);