using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidForceStopApp;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Executors;

public class AndroidForceStopAppExecutor(
    IAndroidDeviceController deviceController,
    ILogger<AndroidForceStopAppExecutor> logger) : IAndroidForceStopAppExecutor
{
    private readonly IAndroidDeviceController _deviceController = deviceController;
    private readonly ILogger<AndroidForceStopAppExecutor> _logger = logger;

    public async Task<AndroidDeviceOperationResult> ExecuteAsync(
        AndroidForceStopAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "force_stopping",
            message: "Force-stopping Android application",
            percent: 0,
            cancellationToken);

        await _deviceController.ForceStopAsync(
            parameters.PackageName,
            parameters.DeviceId,
            cancellationToken);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "Android application force-stopped",
            percent: 100,
            cancellationToken);

        _logger.LogInformation(
            "Android application force-stopped. PackageName={PackageName}, DeviceId={DeviceId}",
            parameters.PackageName,
            parameters.DeviceId);

        return new AndroidDeviceOperationResult(
            Status: "completed",
            Operation: "android-force-stop-app",
            PackageName: parameters.PackageName,
            DeviceId: parameters.DeviceId);
    }
}