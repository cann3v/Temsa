using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Decompile;

namespace Temsa.Worker.StaticAnalysis.Executors;

public class FakeDecompileExecutor(
    ILogger<FakeDecompileExecutor> logger) : IDecompileExecutor
{
    private readonly ILogger<FakeDecompileExecutor> _logger = logger;

    public async Task<DecompileExecutionResult> ExecuteAsync(
        DecompileTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Executing fake decompile using tool {Tool} for input artifact {InputArtifactId}",
            parameters.Tool,
            parameters.InputArtifact.Id);

        await events.ReportProgressAsync(
            phase: "started",
            message: "Fake decompile execution started",
            percent: 0,
            cancellationToken);

        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        await events.ReportProgressAsync(
            phase: "running",
            message: "Fake decompile execution running",
            percent: 50,
            cancellationToken);
        
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "Fake decompile execution completed",
            percent: 100,
            cancellationToken);

        await events.ReportLogAsync(
            message: "analyzed 100 files, found 50 findings",
            level: nameof(LogLevel.Information),
            cancellationToken);

        return new DecompileExecutionResult(
            Tool: parameters.Tool,
            Status: "completed",
            OutputFormat: parameters.OutputFormat,
            GeneratedArtifacts: [],
            Log: null);
    }
}