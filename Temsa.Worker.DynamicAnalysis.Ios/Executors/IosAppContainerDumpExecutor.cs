using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Models.IosAppContainerDump;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Ios.Executors;

public class IosAppContainerDumpExecutor(
    IWorkerDeviceContext deviceContext,
    ILogger<IosAppContainerDumpExecutor> logger) : IIosAppContainerDumpExecutor
{
    private readonly IWorkerDeviceContext _deviceContext = deviceContext;
    private readonly ILogger<IosAppContainerDumpExecutor> _logger = logger;

    public async Task<IosAppContainerDumpExecutionResult> ExecuteAsync(
        IosAppContainerDumpTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "dumping_container",
            message: "iOS app container dump is not implemented yet",
            percent: 0,
            cancellationToken);
        
        // Dump app container and upload to artifact storage
        
        await events.ReportLogAsync(
            message: "iOS app container dump executor placeholder executed",
            level: nameof(LogLevel.Information),
            cancellationToken);

        _logger.LogInformation(
            "iOS app container dump placeholder completed. BundleId={BundleId}, DeviceId={DeviceId}",
            parameters.BundleId,
            _deviceContext.DeviceId);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "iOS app container dump placeholder completed",
            percent: 100,
            cancellationToken);

        return new IosAppContainerDumpExecutionResult(
            Status: "completed",
            BundleId: parameters.BundleId,
            IncludeTmp: parameters.IncludeTmp,
            IncludeCaches: parameters.IncludeCaches,
            Artifacts: []);
    }
}