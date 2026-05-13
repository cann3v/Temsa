using Temsa.Common.Storage;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Sast;

namespace Temsa.Worker.StaticAnalysis.Executors;

public class FakeSastExecutor(
    IArtifactStorage artifactStorage,
    ILogger<FakeSastExecutor> logger) : ISastExecutor
{
    private readonly IArtifactStorage _artifactStorage = artifactStorage;
    private readonly ILogger<FakeSastExecutor> _logger = logger;

    public async Task<SastExecutionResult> ExecuteAsync(
        SastTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Executing fake SAST using tool {Tool} for input artifact {InputArtifactId}",
            parameters.Tool,
            parameters.InputArtifact.Id);

        await events.ReportProgressAsync(
            phase: "started",
            message: "Fake SAST execution started",
            percent: 0,
            cancellationToken);

        var inputArtifact = parameters.InputArtifact;
        var ms = new MemoryStream();
        _logger.LogDebug("Downloading input artifact {InputArtifactId} from bucket {Bucket}, object key {ObjectKey}",
            inputArtifact.Id,
            inputArtifact.Bucket,
            inputArtifact.ObjectKey);
        await _artifactStorage.DownloadAsync(
            inputArtifact.Bucket,
            inputArtifact.ObjectKey,
            ms,
            cancellationToken);

        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "Fake SAST execution completed",
            percent: 100,
            cancellationToken);

        await events.ReportLogAsync(
            message: "Result: 50 findings in 100 files",
            level: nameof(LogLevel.Information),
            cancellationToken);

        return new SastExecutionResult(
            Tool: parameters.Tool,
            Status: "completed",
            Ruleset: parameters.Ruleset,
            ReportFormat: parameters.ReportFormat,
            Threads: parameters.Threads,
            FindingsCount: 50,
            Artifacts: []);
    }
}