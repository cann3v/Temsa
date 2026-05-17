using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidClearAppData;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Handlers;

public class AndroidClearAppDataTaskHandler(
    IAndroidClearAppDataExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<AndroidClearAppDataTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IAndroidClearAppDataExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<AndroidClearAppDataTaskHandler> _logger = logger;

    public string TaskType => "android-clear-app-data";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = DeserializePipelineParameters(context.Task.ParametersJson);

        _logger.LogInformation(
            "Executing Android clear app data task {ScanTaskId} for package {PackageName}",
            context.Task.ScanTaskId,
            parameters.PackageName);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);

        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "Android application data cleared successfully");
    }
    
    private static AndroidClearAppDataTaskParameters DeserializePipelineParameters(string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException("Android clear app data task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<AndroidClearAppDataTaskParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException("Android clear app data task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.PackageName))
        {
            throw new InvalidOperationException("Android clear app data parameter 'packageName' is required");
        }

        return parameters;
    }
}