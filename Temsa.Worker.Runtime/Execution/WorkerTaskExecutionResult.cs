namespace Temsa.Worker.Runtime.Execution;

public record WorkerTaskExecutionResult(
    string? ResultJson,
    string? Message,
    string? Log);