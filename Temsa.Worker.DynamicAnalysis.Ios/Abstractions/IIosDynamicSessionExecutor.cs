using Temsa.Worker.DynamicAnalysis.Ios.Models.IosDynamicSession;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Ios.Abstractions;

public interface IIosDynamicSessionExecutor
{
    Task<IosDynamicSessionExecutionResult> ExecuteAsync(
        IosDynamicSessionTaskParameters parameters,
        WorkerTaskExecutionControl control,
        CancellationToken cancellationToken = default);
}