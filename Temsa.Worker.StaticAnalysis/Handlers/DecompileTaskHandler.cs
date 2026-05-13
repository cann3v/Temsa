using System.Text.Json;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Decompile;

namespace Temsa.Worker.StaticAnalysis.Handlers;

public class DecompileTaskHandler(
    IDecompileExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<DecompileTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IDecompileExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<DecompileTaskHandler> _logger = logger;
    
    public string TaskType => "decompile";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);

        var parameters = new DecompileTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            Platform: context.Task.Platform,
            Tool: context.Task.Tool,
            OutputFormat: pipelineParameters.OutputFormat);
        
        _logger.LogInformation(
            "Execution decompile task {ScanTaskId} using tool {Tool}",
            context.Task.ScanTaskId,
            context.Task.Tool);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);
        
        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Artifacts: result.GeneratedArtifacts,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "Decompile task completed successfully",
            Log: "Fake decompile execution finished");
    }

    private static PipelineDecompileTaskParameters DeserializePipelineParameters(string? parametersJson)
    {
        return string.IsNullOrWhiteSpace(parametersJson)
            ? new PipelineDecompileTaskParameters(null)
            : JsonSerializer.Deserialize<PipelineDecompileTaskParameters>(
                parametersJson,
                JsonSerializerOptions) ?? new PipelineDecompileTaskParameters(null);
    }

    private record PipelineDecompileTaskParameters(
        string? OutputFormat);
}