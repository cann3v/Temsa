using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidDynamicSessionExecutor
{
    Task<AndroidDynamicSessionExecutionResult> ExecuteAsync(
        AndroidDynamicSessionTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}