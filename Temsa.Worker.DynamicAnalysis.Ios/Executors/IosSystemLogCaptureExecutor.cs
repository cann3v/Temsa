using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosSystemLogCapture;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Ios.Executors;

public class IosSystemLogCaptureExecutor(
    IWorkerDeviceContext deviceContext,
    ILogger<IosSystemLogCaptureExecutor> logger) : IIosSystemLogCaptureExecutor
{
    private readonly IWorkerDeviceContext _deviceContext = deviceContext;
    private readonly ILogger<IosSystemLogCaptureExecutor> _logger = logger;

    public async Task<IosSystemLogCaptureExecutionResult> ExecuteAsync(
        IosSystemLogCaptureTaskParameters parameters,
        WorkerTaskExecutionControl control,
        CancellationToken cancellationToken = default)
    {
        await control.Events.ReportProgressAsync(
            phase: "capturing_logs",
            message: "iOS system log capture is not implemented yet",
            percent: 0,
            cancellationToken);
        
        // TODO Log capturing
        
        await control.Events.ReportLogAsync(
            message: "iOS system log capture executor placeholder executed",
            level: nameof(LogLevel.Information),
            cancellationToken);
        
        _logger.LogInformation(
            "iOS system log capture placeholder completed. BundleId={BundleId}, DeviceId={DeviceId}",
            parameters.BundleId,
            _deviceContext.DeviceId);

        await control.Events.ReportProgressAsync(
            phase: "completed",
            message: "iOS system log capture placeholder completed",
            percent: 100,
            cancellationToken);

        return new IosSystemLogCaptureExecutionResult(
            Status: "completed",
            BundleId: parameters.BundleId,
            LinesCount: 0,
            Artifacts: []);

    }
}