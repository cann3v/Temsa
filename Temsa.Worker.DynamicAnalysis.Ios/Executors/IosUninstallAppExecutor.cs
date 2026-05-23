using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosUninstallApp;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Ios.Executors;

public class IosUninstallAppExecutor(
    IWorkerDeviceContext deviceContext,
    ILogger<IosInstallAppExecutor> logger) : IIosUninstallAppExecutor
{
    private readonly IWorkerDeviceContext _deviceContext = deviceContext;
    private readonly ILogger<IosInstallAppExecutor> _logger = logger;

    public async Task<IosDeviceOperationResult> ExecuteAsync(
        IosUninstallAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "uninstalling",
            message: "iOS uninstall app task is not implemented yet",
            percent: 0,
            cancellationToken);
        
        // TODO Uninstall app
        
        await events.ReportLogAsync(
            message: "iOS uninstall app executor placeholder executed",
            level: nameof(LogLevel.Information),
            cancellationToken);

        _logger.LogInformation(
            "iOS uninstall app placeholder completed. BundleId={BundleId}, DeviceId={DeviceId}",
            parameters.BundleId,
            _deviceContext.DeviceId);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "iOS uninstall app placeholder completed",
            percent: 100,
            cancellationToken);

        return new IosDeviceOperationResult(
            Status: "completed",
            Operation: "ios-uninstall-app",
            parameters.BundleId,
            _deviceContext.DeviceId);
    }
}