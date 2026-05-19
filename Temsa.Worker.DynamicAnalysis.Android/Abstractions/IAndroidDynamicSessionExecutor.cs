using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidDynamicSession;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidDynamicSessionExecutor
{
    Task<AndroidDynamicSessionExecutionResult> ExecuteAsync(
        AndroidDynamicSessionTaskParameters parameters,
        WorkerTaskExecutionControl control,
        CancellationToken cancellationToken = default);
}