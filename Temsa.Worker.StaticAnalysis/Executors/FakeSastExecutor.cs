using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Sast;

namespace Temsa.Worker.StaticAnalysis.Executors;

public class FakeSastExecutor(
    ILogger<FakeSastExecutor> logger) : ISastExecutor
{
    private readonly ILogger<FakeSastExecutor> _logger = logger;

    public async Task<SastExecutionResult> ExecuteAsync(
        SastTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Executing fake SAST using tool {Tool} for input artifact {InputArtifactId}",
            parameters.Tool,
            parameters.InputArtifactId);

        await events.ReportProgressAsync(
            phase: "started",
            message: "Fake SAST execution started",
            percent: 0,
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