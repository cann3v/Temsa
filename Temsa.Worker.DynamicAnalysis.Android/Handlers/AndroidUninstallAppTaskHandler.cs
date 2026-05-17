using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidUninstallApp;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Handlers;

public class AndroidUninstallAppTaskHandler(
    IAndroidUninstallAppExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<AndroidUninstallAppTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IAndroidUninstallAppExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<AndroidUninstallAppTaskHandler> _logger = logger;

    public string TaskType => "android-uninstall-app";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = DeserializePipelineParameters(context.Task.ParametersJson);

        _logger.LogInformation(
            "Executing Android uninstall app task {ScanTaskId} for package {PackageName}",
            context.Task.ScanTaskId,
            parameters.PackageName);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);

        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "Android application uninstalled successfully");
    }
    
    private static AndroidUninstallAppTaskParameters DeserializePipelineParameters(
        string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException("Android uninstall app task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<AndroidUninstallAppTaskParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException("Android uninstall app task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.PackageName))
        {
            throw new InvalidOperationException("Android uninstall app parameter 'packageName' is required");
        }

        return parameters;
    }
}