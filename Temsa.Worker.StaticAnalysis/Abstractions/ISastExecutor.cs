using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Sast;

namespace Temsa.Worker.StaticAnalysis.Abstractions;

public interface ISastExecutor
{
    Task<SastExecutionResult> ExecuteAsync(
        SastTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}