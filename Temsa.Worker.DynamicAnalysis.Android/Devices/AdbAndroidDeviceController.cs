using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.Models;
using AdvancedSharpAdbClient.Receivers;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Devices;

public class AdbAndroidDeviceController(
    ILogger<AdbAndroidDeviceController> logger) : IAndroidDeviceController
{
    private readonly IAdbClient _adbClient = AdbClient.Instance;
    private readonly ILogger<AdbAndroidDeviceController> _logger = logger;

    public async Task InstallAsync(
        string apkPath,
        string? deviceId,
        bool reinstall,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apkPath);

        if (!File.Exists(apkPath))
        {
            throw new FileNotFoundException("APK file was not found", apkPath);
        }

        var device = await ResolveDeviceAsync(deviceId, cancellationToken);

        var arguments = reinstall
            ? new[] { "-r" }
            : [];
        
        _logger.LogDebug(
            "Installing APK {ApkPath} on Android device {DeviceSerial}. Reinstall={Reinstall}",
            apkPath,
            device.Serial,
            reinstall);

        await _adbClient.InstallPackageAsync(
            device,
            apkPath,
            progress: null,
            cancellationToken,
            arguments);
        
        _logger.LogInformation(
            "APK {ApkPath} installed on Android device {DeviceSerial}",
            apkPath,
            device.Serial);
    }

    public async Task ClearAppDataAsync(
        string packageName,
        string? deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageName);

        var device = await ResolveDeviceAsync(deviceId, cancellationToken);
        
        _logger.LogDebug(
            "Clearing app data for package {PackageName} on Android device {DeviceSerial}",
            packageName,
            device.Serial);

        var output = await ExecuteShellCommandAsync(
            device,
            $"pm clear {packageName}",
            cancellationToken);

        if (!IsSuccessOutput(output))
        {
            throw new InvalidOperationException(
                $"Failed to clear app data for package '{packageName}'. Output: {output}");
        }

        _logger.LogInformation(
            "App data cleared for package {PackageName} on Android device {DeviceSerial}",
            packageName,
            device.Serial);
    }

    public async Task ForceStopAsync(
        string packageName,
        string? deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageName);
        
        var device =  await ResolveDeviceAsync(deviceId, cancellationToken);
        
        _logger.LogDebug("Force-stopping package {PackageName} on Android device {DeviceSerial}",
            packageName,
            device.Serial);

        await _adbClient.StopAppAsync(
            device,
            packageName,
            cancellationToken);
        
        _logger.LogInformation("Package {PackageName} stopped on Android device {DeviceSerial}",
            packageName,
            device.Serial);
    }

    public async Task UninstallAsync(
        string packageName,
        string? deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageName);

        var device = await ResolveDeviceAsync(deviceId, cancellationToken);
        
        _logger.LogDebug(
            "Uninstalling package {PackageName} from Android device {DeviceSerial}",
            packageName,
            device.Serial);

        await _adbClient.UninstallPackageAsync(
            device,
            packageName,
            cancellationToken);
        
        _logger.LogInformation(
            "Package {PackageName} uninstalled from Android device {DeviceSerial}",
            packageName,
            device.Serial);
    }

    private async Task<DeviceData> ResolveDeviceAsync(
        string? deviceId,
        CancellationToken cancellationToken)
    {
        var devices = (await _adbClient.GetDevicesAsync(cancellationToken))
            .Where(x => x.State == DeviceState.Online)
            .ToArray();

        if (!string.IsNullOrWhiteSpace(deviceId))
        {
            var device = devices.FirstOrDefault(x =>
                string.Equals(x.Serial, deviceId, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.TransportId, deviceId, StringComparison.OrdinalIgnoreCase));

            if (device is null)
            {
                throw new InvalidOperationException(
                    $"Android device '{deviceId}' was not found or is not ready. " +
                    $"Available devices: {FormatDevices(devices)}");
            }

            return device;
        }

        if (devices.Length == 1)
        {
            return devices[0];
        }

        if (devices.Length == 0)
        {
            throw new InvalidOperationException("No online Android devices were found via ADB");
        }

        throw new InvalidOperationException(
            $"Multiple online Android devices were found. Specify deviceId. " +
            $"Available devices: {FormatDevices(devices)}");
    }

    private static string FormatDevices(IReadOnlyCollection<DeviceData> devices)
    {
        if (devices.Count == 0)
        {
            return "<none>";
        }

        return string.Join(
            ", ",
            devices.Select(x =>
                $"{x.Serial} state={x.State} model={x.Model} product={x.Product} transportId={x.TransportId}"));
    }

    private async Task<string> ExecuteShellCommandAsync(
        DeviceData device,
        string command,
        CancellationToken cancellationToken)
    {
        var receiver = new ConsoleOutputReceiver();

        await _adbClient.ExecuteShellCommandAsync(
            device,
            command,
            receiver,
            cancellationToken);

        var output = receiver.ToString();
        
        _logger.LogDebug(
            "ADB shell command completed on {DeviceSerial}. Command={Command}; Output={Output}",
            device.Serial,
            command,
            output);

        return output;
    }

    private static bool IsSuccessOutput(string output)
    {
        return output
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(x => string.Equals(x, "Success", StringComparison.OrdinalIgnoreCase));
    }
}