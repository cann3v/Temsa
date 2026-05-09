using System.Text.Json;
using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.Runtime.Execution;

public record WorkerTaskExecutionResult(
    string Status = "completed",
    IReadOnlyCollection<ScanArtifactDescriptor>? Artifacts = null,
    JsonElement? Result = null,
    string? Message = null,
    string? Log = null)
{
    public static WorkerTaskExecutionResult Empty { get; } = new();
}