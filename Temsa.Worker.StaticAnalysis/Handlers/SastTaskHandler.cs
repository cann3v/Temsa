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
            InputArtifact: context.Task.InputArtifact,
            Platform: context.Task.Platform,
            Tool: context.Task.Tool,
            Ruleset: pipelineParameters.Ruleset,
            ReportFormat: pipelineParameters.ReportFormat,
            Threads: pipelineParameters.Threads,
            Radare2Profile: pipelineParameters.Radare2Profile,
            EnabledRadare2Scripts: pipelineParameters.EnabledRadare2Scripts,
            DisabledRadare2Scripts: pipelineParameters.DisabledRadare2Scripts,
            EnabledRadare2Analyzers: pipelineParameters.EnabledRadare2Analyzers,
            DisabledRadare2Analyzers: pipelineParameters.DisabledRadare2Analyzers);

        _logger.LogInformation(
            "Executing SAST task {ScanTaskId} using tool {Tool}",
            context.Task.ScanTaskId,
            context.Task.Tool);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);

        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Artifacts: result.Artifacts,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "SAST task completed successfully",
            Log: "SAST execution finished");
    }

    private static PipelineSastTaskParameters DeserializePipelineParameters(string? parametersJson)
    {
        return string.IsNullOrWhiteSpace(parametersJson)
            ? new PipelineSastTaskParameters(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null)
            : JsonSerializer.Deserialize<PipelineSastTaskParameters>(
                  parametersJson,
                  JsonSerializerOptions) ??
              new PipelineSastTaskParameters(
                  null,
                  null,
                  null,
                  null,
                  null,
                  null,
                  null,
                  null);
    }

    private record PipelineSastTaskParameters(
        string? Ruleset,
        string? ReportFormat,
        int? Threads,
        string? Radare2Profile,
        IReadOnlyCollection<string>? EnabledRadare2Scripts,
        IReadOnlyCollection<string>? DisabledRadare2Scripts,
        IReadOnlyCollection<string>? EnabledRadare2Analyzers,
        IReadOnlyCollection<string>? DisabledRadare2Analyzers);
}