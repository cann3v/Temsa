using System.Text.Json;

namespace Temsa.Worker.Runtime.Execution;

public record WorkerTaskExecutionResult(
    JsonElement? Payload = null,
    string? Message = null,
    string? Log = null)
{
    public static WorkerTaskExecutionResult Empty { get; } = new();
}