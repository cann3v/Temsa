using System.Text.Json;
using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosDynamicSession;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Ios.Handlers;

public class IosDynamicSessionTaskHandler(
    IIosDynamicSessionExecutor executor,
    IWorkerTaskEventSinkFactory eventSinkFactory,
    ILogger<IosDynamicSessionTaskHandler> logger) : IWorkerTaskHandler
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IIosDynamicSessionExecutor _executor = executor;
    private readonly IWorkerTaskEventSinkFactory _eventSinkFactory = eventSinkFactory;
    private readonly ILogger<IosDynamicSessionTaskHandler> _logger = logger;

    public string TaskType => "ios-dynamic-session";

    public async Task<WorkerTaskExecutionResult> ExecuteAsync(
        WorkerTaskContext context,
        CancellationToken cancellationToken = default)
    {
        var pipelineParameters = DeserializePipelineParameters(context.Task.ParametersJson);
        
        var parameters = new IosDynamicSessionTaskParameters(
            InputArtifact: context.Task.InputArtifact,
            BundleId: pipelineParameters.BundleId,
            SessionTimeoutSeconds: pipelineParameters.SessionTimeoutSeconds,
            ScriptProfile: pipelineParameters.ScriptProfile,
            EnabledScripts: pipelineParameters.EnabledScripts ?? [],
            DisabledScripts: pipelineParameters.DisabledScripts ?? []);
        
        _logger.LogInformation(
            "Executing iOS dynamic session task {ScanTaskId} for bundle {BundleId}",
            context.Task.ScanTaskId,
            parameters.BundleId);
        
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
            Message: "iOS dynamic session completed successfully",
            Log: "iOS dynamic session execution finished");
    }
    
    private static IosDynamicSessionPipelineParameters DeserializePipelineParameters(string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
        {
            throw new InvalidOperationException("iOS dynamic session task parameters are empty");
        }

        var parameters = JsonSerializer.Deserialize<IosDynamicSessionPipelineParameters>(
            parametersJson,
            JsonSerializerOptions);

        if (parameters is null)
        {
            throw new InvalidOperationException("iOS dynamic session task parameters are invalid");
        }

        if (string.IsNullOrWhiteSpace(parameters.BundleId))
        {
            throw new InvalidOperationException("iOS dynamic session parameter 'bundleId' is required");
        }

        if (string.IsNullOrWhiteSpace(parameters.ScriptProfile))
        {
            throw new InvalidOperationException("iOS dynamic session parameter 'scriptProfile' is required");
        }

        return parameters;
    }
}