using System.Text.Json;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Sast;

namespace Temsa.Worker.StaticAnalysis.Handlers;

public class SastTaskHandler(
    ISastExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<SastTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly ISastExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<SastTaskHandler> _logger = logger;
    
    public string TaskType => "sast";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);
        
        var parameters = new SastTaskParameters(
            InputArtifactId:  context.Task.InputArtifactId,
            Platform: context.Task.Platform,
            Tool: context.Task.Tool,
            Ruleset: pipelineParameters.Ruleset,
            ReportFormat: pipelineParameters.ReportFormat,
            Threads: pipelineParameters.Threads);
        
        _logger.LogInformation(
            "Executing SAST task {ScanTaskId} using tool {Tool}",
            context.Task.ScanTaskId,
            context.Task.Tool);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);

        return new WorkerTaskExecutionResult(
            ResultJson: JsonSerializer.Serialize(result, JsonSerializerOptions),
            Message: "SAST task completed successfully",
            Log: "Fake SAST execution finished");
    }

    private static PipelineSastTaskParameters DeserializePipelineParameters(string? parametersJson)
    {
        return string.IsNullOrWhiteSpace(parametersJson)
            ? new PipelineSastTaskParameters(null, null, null)
            : JsonSerializer.Deserialize<PipelineSastTaskParameters>(
                parametersJson,
                JsonSerializerOptions) ?? new PipelineSastTaskParameters(null, null, null);
    }

    private record PipelineSastTaskParameters(
        string? Ruleset,
        string? ReportFormat,
        int? Threads);
}