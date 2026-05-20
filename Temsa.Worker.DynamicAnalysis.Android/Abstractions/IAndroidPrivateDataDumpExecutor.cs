using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidPrivateDataDump;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidPrivateDataDumpExecutor
{
    Task<AndroidPrivateDataDumpExecutionResult> ExecuteAsync(
        AndroidPrivateDataDumpTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}