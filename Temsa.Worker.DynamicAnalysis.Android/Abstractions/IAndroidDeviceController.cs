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
}