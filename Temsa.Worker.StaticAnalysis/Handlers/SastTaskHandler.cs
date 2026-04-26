using System.Text.Json;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.StaticAnalysis.Handlers;

public class SastTaskHandler : IWorkerTaskHandler
{
    public string TaskType => "sast";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        context.Logger.LogInformation(
            "Executing SAST task {ScanTaskId} using tool {Tool}",
            context.Task.ScanTaskId,
            context.Task.Tool);

        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        var resultJson = JsonSerializer.Serialize(new
        {
            tool = context.Task.Tool,
            status = "completed",
            findingsCount = 0,
            findings = Array.Empty<object>()
        });

        return new WorkerTaskExecutionResult(
            ResultJson: resultJson,
            Message: "SAST task completed successfully",
            Log: "Fake SAST execution finished");
    }
}