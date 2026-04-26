using System.Text.Json;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.StaticAnalysis.Handlers;

public class DecompileTaskHandler : IWorkerTaskHandler
{
    public string TaskType => "decompile";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        context.Logger.LogInformation(
            "Execution decompile task {ScanTaskId} using tool {Tool}",
            context.Task.ScanTaskId,
            context.Task.Tool);

        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        var resultJson = JsonSerializer.Serialize(new
        {
            tool = context.Task.Tool,
            status = "completed",
            generatedArtifacts = Array.Empty<object>()
        });

        return new WorkerTaskExecutionResult(
            ResultJson: resultJson,
            Message: "Decompile task completed successfully",
            Log: "Fake decompile execution finished");
    }
}