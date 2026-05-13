using Temsa.Contracts.Artifacts;

namespace Temsa.Contracts.Messaging.ScanTasks;

public record ScanTaskDispatchMessage(
    long ScanTaskId,
    long ScanId,
    ProjectArtifactDescriptor InputArtifact,
    string Platform,
    string TaskType,
    string Tool,
    string? ParametersJson);