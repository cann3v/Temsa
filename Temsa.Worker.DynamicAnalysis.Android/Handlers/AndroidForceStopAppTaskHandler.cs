using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidForceStopApp;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Handlers;

public class AndroidForceStopAppTaskHandler(
    IAndroidForceStopAppExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<AndroidForceStopAppTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IAndroidForceStopAppExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<AndroidForceStopAppTaskHandler> _logger = logger;

    public string TaskType => "android-force-stop-app";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = DeserializePipelineParameters(context.Task.ParametersJson);

        _logger.LogInformation(
            "Executing Android force-stop app task {ScanTaskId} for package {PackageName}",
            context.Task.ScanTaskId,
            parameters.PackageName);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);

        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "Android application force-stopped successfully");
    }
    
    private static AndroidForceStopAppTaskParameters DeserializePipelineParameters(
        string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException("Android force-stop app task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<AndroidForceStopAppTaskParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException("Android force-stop app task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.PackageName))
        {
            throw new InvalidOperationException("Android force-stop app parameter 'packageName' is required");
        }

        return parameters;
    }
}