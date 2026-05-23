using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosForceStopApp;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Ios.Executors;

public class IosForceStopAppExecutor(
    IWorkerDeviceContext deviceContext,
    ILogger<IosInstallAppExecutor> logger) : IIosForceStopAppExecutor
{
    private readonly IWorkerDeviceContext _deviceContext = deviceContext;
    private readonly ILogger<IosInstallAppExecutor> _logger = logger;

    public async Task<IosDeviceOperationResult> ExecuteAsync(
        IosForceStopAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "stopping",
            message: "iOS force stop app task is not implemented yet",
            percent: 0,
            cancellationToken);
        
        // TODO Stop app
        
        await events.ReportLogAsync(
            message: "iOS force stop app executor placeholder executed",
            level: nameof(LogLevel.Information),
            cancellationToken);

        _logger.LogInformation(
            "iOS force stop app placeholder completed. BundleId={BundleId}, DeviceId={DeviceId}",
            parameters.BundleId,
            _deviceContext.DeviceId);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "iOS force stop app placeholder completed",
            percent: 100,
            cancellationToken);

        return new IosDeviceOperationResult(
            Status: "completed",
            Operation: "ios-force-stop-app",
            parameters.BundleId,
            _deviceContext.DeviceId);
    }
}