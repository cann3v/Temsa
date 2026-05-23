using Temsa.Worker.DynamicAnalysis.Ios.Models;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosInstallApp;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Ios.Abstractions;

public interface IIosInstallAppExecutor
{
    Task<IosDeviceOperationResult> ExecuteAsync(
        IosInstallAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default);
}