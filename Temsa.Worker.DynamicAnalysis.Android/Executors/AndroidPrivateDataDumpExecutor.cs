using Temsa.Common.Storage;
using Temsa.Contracts.Artifacts;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidPrivateDataDump;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Executors;

public class AndroidPrivateDataDumpExecutor(
    IAndroidDeviceController deviceController,
    IArtifactStorage artifactStorage,
    IWorkerDeviceContext deviceContext,
    ILogger<AndroidPrivateDataDumpExecutor> logger) : IAndroidPrivateDataDumpExecutor
{
    private readonly IAndroidDeviceController _deviceController = deviceController;
    private readonly IArtifactStorage _artifactStorage = artifactStorage;
    private readonly IWorkerDeviceContext _deviceContext = deviceContext;
    private readonly ILogger<AndroidPrivateDataDumpExecutor> _logger = logger;

    public async Task<AndroidPrivateDataDumpExecutionResult> ExecuteAsync(
        AndroidPrivateDataDumpTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "dumping_private_data",
            message: "Dumping Android application private data",
            percent: 0,
            cancellationToken);

        var tempDirectory = CreateTempDirectory();
        var archivePath = Path.Combine(tempDirectory, $"{parameters.PackageName}-private-data.tar");

        try
        {
            await _deviceController.DumpPrivateDataAsync(
                parameters.PackageName,
                _deviceContext.DeviceId,
                archivePath,
                parameters.IncludeCache,
                cancellationToken);

            await events.ReportProgressAsync(
                phase: "uploading_private_data_dump",
                message: "Uploading Android private data dump artifact",
                percent: 80,
                cancellationToken);

            await using var archiveStream = File.OpenRead(archivePath);

            var objectKey = $"dynamic/android/{parameters.InputArtifact.Id}/" +
                            $"{Guid.NewGuid():N}/{parameters.PackageName}-private-data.tar";


            var storedArchive = await _artifactStorage.UploadAsync(
                archiveStream,
                objectKey,
                fileName: $"{parameters.PackageName}-private-data.tar",
                contentType: "application/x-tar",
                cancellationToken);

            var artifact = new ScanArtifactDescriptor(
                Kind: ArtifactKind.Other,
                Bucket: storedArchive.Bucket,
                ObjectKey: storedArchive.ObjectKey,
                FileName: storedArchive.FileName,
                ContentType: storedArchive.ContentType,
                SizeBytes: storedArchive.SizeBytes);

            await events.ReportProgressAsync(
                phase: "completed",
                message: "Android private data dump completed",
                percent: 100,
                cancellationToken);

            _logger.LogInformation(
                "Android private data dump completed. " +
                "PackageName={PackageName}, IncludeCache={IncludeCache}, SizeBytes={SizeBytes}",
                parameters.PackageName,
                parameters.IncludeCache,
                storedArchive.SizeBytes);

            return new AndroidPrivateDataDumpExecutionResult(
                Status: "completed",
                PackageName: parameters.PackageName,
                IncludeCache: parameters.IncludeCache,
                Artifacts: [artifact]);
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
            $"temsa-android-private-data-{Guid.NewGuid():N}");

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
        catch { }
    }
}