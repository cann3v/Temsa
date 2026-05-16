using System.Text.Json;
using Microsoft.Extensions.Logging;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Handlers;

public class AndroidDynamicSessionTaskHandler(
    IAndroidDynamicSessionExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<AndroidDynamicSessionTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IAndroidDynamicSessionExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<AndroidDynamicSessionTaskHandler> _logger = logger;

    public string TaskType => "android-dynamic-session";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);

        var parameters = new AndroidDynamicSessionTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            PackageName: pipelineParameters.PackageName,
            DeviceId: pipelineParameters.DeviceId,
            SessionTimeoutSeconds: pipelineParameters.SessionTimeoutSeconds,
            ScriptProfile: pipelineParameters.ScriptProfile,
            EnabledScripts: pipelineParameters.EnabledScripts ?? [],
            DisabledScripts: pipelineParameters.DisabledScripts ?? []);
        
        _logger.LogInformation(
            "Executing Android dynamic session task {ScanTaskId} for package {PackageName}",
            context.Task.ScanTaskId,
            parameters.PackageName);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);

        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Artifacts: result.Artifacts,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "Android dynamic session completed successfully",
            Log: "Android dynamic session execution finished");
    }

    private static AndroidDynamicSessionPipelineParameters DeserializePipelineParameters(
        string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException(
                "Android dynamic session task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<AndroidDynamicSessionPipelineParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException(
                "Android dynamic session task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.PackageName))
        {
            throw new InvalidOperationException(
                "Android dynamic session parameter 'packageName' is required");
        }

        if (string.IsNullOrWhiteSpace(parameters.ScriptProfile))
        {
            throw new InvalidOperationException(
                "Android dynamic session parameters 'scriptProfile' is required");
        }

        return parameters;
    }
}