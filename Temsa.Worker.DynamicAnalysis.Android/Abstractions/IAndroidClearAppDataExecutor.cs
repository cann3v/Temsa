using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidClearAppData;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidClearAppDataExecutor
{
    Task<AndroidDeviceOperationResult> ExecuteAsync(
        AndroidClearAppDataTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}