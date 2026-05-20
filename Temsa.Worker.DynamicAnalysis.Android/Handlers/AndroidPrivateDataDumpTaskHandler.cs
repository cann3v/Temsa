using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidPrivateDataDump;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Handlers;

public class AndroidPrivateDataDumpTaskHandler(
    IAndroidPrivateDataDumpExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<AndroidPrivateDataDumpTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IAndroidPrivateDataDumpExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<AndroidPrivateDataDumpTaskHandler> _logger = logger;

    public string TaskType => "android-private-data-dump";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);

        var parameters = new AndroidPrivateDataDumpTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            PackageName: pipelineParameters.PackageName,
            IncludeCache: pipelineParameters.IncludeCache);

        _logger.LogInformation(
            "Executing Android private data dump task {ScanTaskId} for package {PackageName}",
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
            Message: "Android private data dump completed successfully",
            Log: "Android private data dump execution finished");
    }
    
    private static AndroidPrivateDataDumpPipelineParameters DeserializePipelineParameters(
        string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException("Android private data dump task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<AndroidPrivateDataDumpPipelineParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException("Android private data dump task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.PackageName))
        {
            throw new InvalidOperationException(
                "Android private data dump parameter 'packageName' is required");
        }

        return parameters;
    }
}