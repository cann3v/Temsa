using Temsa.Worker.DynamicAnalysis.Ios.Models.IosAppContainerDump;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Ios.Abstractions;

public interface IIosAppContainerDumpExecutor
{
    Task<IosAppContainerDumpExecutionResult> ExecuteAsync(
        IosAppContainerDumpTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}