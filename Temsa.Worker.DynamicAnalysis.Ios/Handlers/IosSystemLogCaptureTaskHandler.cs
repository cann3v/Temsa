using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosSystemLogCapture;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Ios.Handlers;

public class IosSystemLogCaptureTaskHandler(
    IIosSystemLogCaptureExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<IosSystemLogCaptureTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IIosSystemLogCaptureExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<IosSystemLogCaptureTaskHandler> _logger = logger;

    public string TaskType => "ios-system-log-capture";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);

        var parameters = new IosSystemLogCaptureTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            BundleId: pipelineParameters.BundleId,
            SessionTimeoutSeconds: pipelineParameters.SessionTimeoutSeconds);
        
        _logger.LogInformation(
            "Executing iOS system log capture task {ScanTaskId}. BundleId={BundleId}",
            context.Task.ScanTaskId,
            parameters.BundleId);

        var control = new WorkerTaskExecutionControl(
            Events: _eventSinkFactory.Create(context),
            StopHandle: context.StopHandle);

        var result = await _executor.ExecuteAsync(parameters, control, cancellationToken);
        
        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Artifacts: result.Artifacts,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "iOS system log capture task completed successfully",
            Log: "iOS system log capture task execution finished");
    }
    
    private static IosSystemLogCapturePipelineParameters DeserializePipelineParameters(
        string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException(
                "iOS system log capture task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<IosSystemLogCapturePipelineParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException(
                "iOS system log capture task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.BundleId))
        {
            throw new InvalidOperationException(
                "iOS system log capture parameter 'bundleId' is required");
        }

        return parameters;
    }

}