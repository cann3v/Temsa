using Temsa.Worker.DynamicAnalysis.Ios.Models;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosForceStopApp;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Ios.Abstractions;

public interface IIosForceStopAppExecutor
{
    Task<IosDeviceOperationResult> ExecuteAsync(
        IosForceStopAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}