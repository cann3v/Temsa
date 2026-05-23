using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosInstallApp;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Ios.Handlers;

public class IosInstallAppTaskHandler(
    IIosInstallAppExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<IosInstallAppTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IIosInstallAppExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<IosInstallAppTaskHandler> _logger = logger;

    public string TaskType => "ios-install-app";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters  = DeserializePipelineParameters(context.Task.ParametersJson);
        
        var parameters = new IosInstallAppTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            BundleId: pipelineParameters.BundleId);

        _logger.LogInformation(
            "Executing iOS install app task {ScanTaskId}. BundleId={BundleId}",
            context.Task.ScanTaskId,
            parameters.BundleId);

        var result = await _executor.ExecuteAsync(
            parameters,
            _eventSinkFactory.Create(context),
            cancellationToken);

        return new WorkerTaskExecutionResult(
            Status: result.Status,
            Result: JsonSerializer.SerializeToElement(result, JsonSerializerOptions),
            Message: "iOS install app task completed successfully",
            Log: "iOS install app task execution finished");
    }
    
    private static IosInstallAppPipelineParameters DeserializePipelineParameters(string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException(
                "iOS install app task parameters are empty");
        }
        
        var parameters = JsonSerializer.Deserialize<IosInstallAppPipelineParameters>(
            parametersJson,
            JsonSerializerOptions);
        
        if (parameters is null)
        {
            throw new InvalidOperationException(
                "iOS install app task parameters are invalid");
        }
        
        if (string.IsNullOrWhiteSpace(parameters.BundleId))
        {
            throw new InvalidOperationException(
                "iOS install app parameter 'bundleId' is required"); // Actually not required
        }

        return parameters;
    }
}