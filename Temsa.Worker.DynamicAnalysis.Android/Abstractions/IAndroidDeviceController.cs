namespace Temsa.Worker.DynamicAnalysis.Android.Abstractions;

public interface IAndroidDeviceController
{
    Task InstallAsync(
        string apkPath,
        string? deviceId,
        bool reinstall,
        CancellationToken cancellationToken = default);

    Task ClearAppDataAsync(
        string packageName,
        string? deviceId,
        CancellationToken cancellationToken = default);
    
    Task ForceStopAsync(
        string packageName,
        string? deviceId,
        CancellationToken cancellationToken = default);
    
    Task UninstallAsync(
        string packageName,
        string? deviceId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<int>> GetProcessIdsAsync(
        string packageName,
        string? deviceId,
        CancellationToken cancellationToken = default);
    
    Task CaptureLogcatAsync(
        int processId,
        string? deviceId,
        Func<string, CancellationToken, Task> onLine,
        CancellationToken cancellationToken = default);
    
    Task DumpPrivateDataAsync(
        string packageName,
        string? deviceId,
        string destinationArchivePath,
        bool includeCache,
        CancellationToken cancellationToken = default);
}