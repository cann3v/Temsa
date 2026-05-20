using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidLogcatCapture;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidLogcatCaptureExecutor
{

    Task<AndroidLogcatCaptureExecutionResult> ExecuteAsync(
        AndroidLogcatCaptureTaskParameters parameters,
        WorkerTaskExecutionControl control,
        CancellationToken cancellationToken = default);
}