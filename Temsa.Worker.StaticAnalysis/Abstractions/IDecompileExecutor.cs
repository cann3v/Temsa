using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;
using Temsa.Worker.StaticAnalysis.Models.Decompile;

namespace Temsa.Worker.StaticAnalysis.Abstractions;

public interface IDecompileExecutor
{
    Task<DecompileExecutionResult> ExecuteAsync(
        DecompileTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}