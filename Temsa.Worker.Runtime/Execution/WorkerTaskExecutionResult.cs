namespace Temsa.Worker.Runtime.Execution;

public record WorkerTaskExecutionResult(
    string? ResultJson = null,
    string? Message = null,
    string? Log = null)
{
    public static WorkerTaskExecutionResult Empty { get; } = new();
}