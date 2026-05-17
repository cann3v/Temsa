using Temsa.Common.Storage;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidInstallApp;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Executors;

public class AndroidInstallAppExecutor(
    IArtifactStorage artifactStorage,
    IAndroidDeviceController deviceController,
    ILogger<AndroidInstallAppExecutor> logger) : IAndroidInstallAppExecutor
{
    private readonly IArtifactStorage _artifactStorage = artifactStorage;
    private readonly IAndroidDeviceController _deviceController = deviceController;
    private readonly ILogger<AndroidInstallAppExecutor> _logger = logger;

    public async Task<AndroidDeviceOperationResult> ExecuteAsync(
        AndroidInstallAppTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        var tempDirectory = CreateTempDirectory();
        
        try
        {
            await events.ReportProgressAsync(
                phase: "input_download",
                message: "Downloading APK from artifact storage",
                percent: 0,
                cancellationToken);

            var apkPath = Path.Combine(
                tempDirectory,
                parameters.InputArtifact.FileName ?? "input.apk");

            await using (var apkFile = File.Create(apkPath))
            {
                await _artifactStorage.DownloadAsync(
                    parameters.InputArtifact.Bucket,
                    parameters.InputArtifact.ObjectKey,
                    apkFile,
                    cancellationToken);
            }

            await events.ReportProgressAsync(
                phase: "installing",
                message: "Installing APK on Android device",
                percent: 50,
                cancellationToken);

            await _deviceController.InstallAsync(
                apkPath,
                parameters.DeviceId,
                parameters.Reinstall,
                cancellationToken);

            await events.ReportProgressAsync(
                phase: "completed",
                message: "APK installed on Android device",
                percent: 100,
                cancellationToken);

            _logger.LogInformation(
                "Android APK installed. FileName={FileName}, DeviceId={DeviceId}, Reinstall={Reinstall}",
                parameters.InputArtifact.FileName,
                parameters.DeviceId,
                parameters.Reinstall);

            return new AndroidDeviceOperationResult(
                Status: "completed",
                Operation: "android-install-app",
                DeviceId: parameters.DeviceId);
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }
    
    private static string CreateTempDirectory()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            $"temsa-android-install-{Guid.NewGuid():N}");

        Directory.CreateDirectory(path);
        return path;
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch
        {
            // ??
        }
    }
}