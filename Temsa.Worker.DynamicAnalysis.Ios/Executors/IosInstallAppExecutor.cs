using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosInstallApp;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Ios.Executors;

public class IosInstallAppExecutor(
    IWorkerDeviceContext deviceContext,
    ILogger<IosInstallAppExecutor> logger) : IIosInstallAppExecutor
{
    private readonly IWorkerDeviceContext _deviceContext = deviceContext;
    private readonly ILogger<IosInstallAppExecutor> _logger = logger;

    public async Task<IosDeviceOperationResult> ExecuteAsync(
        IosInstallAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "installing",
            message: "iOS install app task is not implemented yet",
            percent: 0,
            cancellationToken);
        
        // TODO Installation
        
        await events.ReportLogAsync(
            message: "iOS install app executor placeholder executed",
            level: nameof(LogLevel.Information),
            cancellationToken);

        _logger.LogInformation(
            "iOS install app placeholder completed. BundleId={BundleId}, DeviceId={DeviceId}",
            parameters.BundleId,
            _deviceContext.DeviceId);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "iOS install app placeholder completed",
            percent: 100,
            cancellationToken);

        return new IosDeviceOperationResult(
            Status: "completed",
            Operation: "ios-install-app",
            parameters.BundleId,
            _deviceContext.DeviceId);
    }
}