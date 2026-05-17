using System.Text.Json;
using Temsa.Common.Storage;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidInstallApp;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Handlers;

public class AndroidInstallAppTaskHandler(
    IAndroidInstallAppExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<AndroidInstallAppTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    
    private readonly IAndroidInstallAppExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<AndroidInstallAppTaskHandler> _logger = logger;

    public string TaskType => "android-install-app";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);

        var parameters = new AndroidInstallAppTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            DeviceId: pipelineParameters.DeviceId,
            Reinstall: pipelineParameters.Reinstall);
        
        _logger.LogInformation(
            "Executing Android install app task {ScanTaskId}",
            context.Task.ScanTaskId);
        
        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);
        
        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "Android APK installed successfully");
    }
    
    private static AndroidInstallAppPipelineParameters DeserializePipelineParameters(
        string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            return new AndroidInstallAppPipelineParameters(
                DeviceId: null,
                Reinstall: true);
        }

        return JsonSerializer.Deserialize<AndroidInstallAppPipelineParameters>(
                   parametersJson,
                   JsonSerializerOptions)
               ?? throw new InvalidOperationException("Android install app task parameters are invalid");
    }
}