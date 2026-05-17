using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidUninstallApp;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidUninstallAppExecutor
{
    Task<AndroidDeviceOperationResult> ExecuteAsync(
        AndroidUninstallAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}