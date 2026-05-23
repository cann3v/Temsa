using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosAppContainerDump;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Ios.Handlers;

public class IosAppContainerDumpTaskHandler(
    IIosAppContainerDumpExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<IosSystemLogCaptureTaskHandler> logger) :IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IIosAppContainerDumpExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<IosSystemLogCaptureTaskHandler> _logger = logger;

    public string TaskType => "ios-app-container-dump";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);

        var parameters = new IosAppContainerDumpTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            BundleId: pipelineParameters.BundleId,
            IncludeTmp: pipelineParameters.IncludeTmp,
            IncludeCaches: pipelineParameters.IncludeCaches);

        _logger.LogInformation(
            "Executing iOS app container dump task {ScanTaskId}. BundleId={BundleId}",
            context.Task.ScanTaskId,
            parameters.BundleId);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);
        
        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Artifacts: result.Artifacts,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "iOS system log capture task completed successfully",
            Log: "iOS system log capture task execution finished");
    }
    
    private static IosAppContainerDumpPipelineParameters DeserializePipelineParameters(string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException(
                "iOS app container dump task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<IosAppContainerDumpPipelineParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException(
                "iOS app container dump task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.BundleId))
        {
            throw new InvalidOperationException(
                "iOS app container dump parameter 'bundleId' is required");
        }

        return parameters;
    }
}