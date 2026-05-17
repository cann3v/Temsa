using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidUninstallApp;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Executors;

public class AndroidUninstallAppExecutor(
    IAndroidDeviceController deviceController,
    ILogger<AndroidUninstallAppExecutor> logger) : IAndroidUninstallAppExecutor
{
    private readonly IAndroidDeviceController _deviceController = deviceController;
    private readonly ILogger<AndroidUninstallAppExecutor> _logger = logger;
    
    public async Task<AndroidDeviceOperationResult> ExecuteAsync(
        AndroidUninstallAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "uninstalling",
            message: "Uninstalling Android application",
            percent: 0,
            cancellationToken);

        await _deviceController.UninstallAsync(
            parameters.PackageName,
            parameters.DeviceId,
            cancellationToken);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "Android application uninstalled",
            percent: 100,
            cancellationToken);

        _logger.LogInformation(
            "Android application uninstalled. PackageName={PackageName}, DeviceId={DeviceId}",
            parameters.PackageName,
            parameters.DeviceId);

        return new AndroidDeviceOperationResult(
            Status: "completed",
            Operation: "android-uninstall-app",
            PackageName: parameters.PackageName,
            DeviceId: parameters.DeviceId);
    }
}