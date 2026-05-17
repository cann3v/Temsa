using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidForceStopApp;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidForceStopAppExecutor
{
    Task<AndroidDeviceOperationResult> ExecuteAsync(
        AndroidForceStopAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}