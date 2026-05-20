using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidLogcatCapture;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Handlers;

public class AndroidLogcatCaptureTaskHandler(
    IAndroidLogcatCaptureExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<AndroidLogcatCaptureTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IAndroidLogcatCaptureExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<AndroidLogcatCaptureTaskHandler> _logger = logger;

    public string TaskType => "android-logcat-capture";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);

        var parameters = new AndroidLogcatCaptureTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            PackageName: pipelineParameters.PackageName,
            SessionTimeoutSeconds: pipelineParameters.SessionTimeoutSeconds,
            ProcessWaitTimeoutSeconds: pipelineParameters.ProcessWaitTimeoutSeconds,
            ProcessPollIntervalMilliseconds: pipelineParameters.ProcessPollIntervalMilliseconds);
        
        _logger.LogInformation(
            "Executing Android logcat capture task {ScanTaskId} for package {PackageName}",
            context.Task.ScanTaskId,
            parameters.PackageName);
        
        var control = new WorkerTaskExecutionControl(
            Events: _eventSinkFactory.Create(context),
            StopHandle: context.StopHandle);

        var result = await _executor.ExecuteAsync(
            parameters,
            control,
            cancellationToken);
        
        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Artifacts: result.Artifacts,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "Android logcat capture completed successfully",
            Log: "Android logcat capture execution finished");
    }
    
    private static AndroidLogcatCapturePipelineParameters DeserializePipelineParameters(
        string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException(
                "Android logcat capture task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<AndroidLogcatCapturePipelineParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException(
                "Android logcat capture task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.PackageName))
        {
            throw new InvalidOperationException(
                "Android logcat capture parameter 'packageName' is required");
        }

        return parameters;
    }
}