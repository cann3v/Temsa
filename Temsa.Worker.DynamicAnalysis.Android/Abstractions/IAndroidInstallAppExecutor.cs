using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidInstallApp;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidInstallAppExecutor
{
    Task<AndroidDeviceOperationResult> ExecuteAsync(
        AndroidInstallAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}