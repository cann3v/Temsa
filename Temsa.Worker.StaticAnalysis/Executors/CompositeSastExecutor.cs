using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Sast;

namespace Temsa.Worker.StaticAnalysis.Executors;

public class CompositeSastExecutor(
    JadxSemgrepSastExecutor jadxSemgrepExecutor,
    TruffleHogRadare2SastExecutor truffleHogRadare2Executor) : ISastExecutor
{
    public Task<SastExecutionResult> ExecuteAsync(
        SastTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        return parameters.Tool.ToLowerInvariant() switch
        {
            "jadx-semgrep" => jadxSemgrepExecutor.ExecuteAsync(parameters, events, cancellationToken),
            "trufflehog-radare2" => truffleHogRadare2Executor.ExecuteAsync(parameters, events, cancellationToken),

            _ => throw new InvalidOperationException(
                $"No SAST executor registered for tool '{parameters.Tool}' and platform '{parameters.Platform}'")
        };
    }
}