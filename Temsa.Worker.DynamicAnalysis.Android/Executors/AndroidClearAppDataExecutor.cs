using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidClearAppData;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Executors;

public class AndroidClearAppDataExecutor(
    IAndroidDeviceController deviceController,
    IWorkerDeviceContext deviceContext,
    ILogger<AndroidClearAppDataExecutor> logger) : IAndroidClearAppDataExecutor
{
    private readonly IAndroidDeviceController _deviceController = deviceController;
    private readonly IWorkerDeviceContext _deviceContext = deviceContext;
    private readonly ILogger<AndroidClearAppDataExecutor> _logger = logger;

    public async Task<AndroidDeviceOperationResult> ExecuteAsync(
        AndroidClearAppDataTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "clearing",
            message: "Clearing Android application data",
            percent: 0,
            cancellationToken);

        await _deviceController.ClearAppDataAsync(
            parameters.PackageName,
            _deviceContext.DeviceId,
            cancellationToken);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "Android application data cleared",
            percent: 100,
            cancellationToken);

        _logger.LogInformation(
            "Android application data cleared. PackageName={PackageName}, DeviceId={DeviceId}",
            parameters.PackageName,
            _deviceContext.DeviceId);

        return new AndroidDeviceOperationResult(
            Status: "completed",
            Operation: "android-clear-app-data",
            PackageName: parameters.PackageName,
            DeviceId: _deviceContext.DeviceId);
    }
}