using System.Diagnostics;
using System.Drawing;
using System.Text;
using Temsa.Common.Storage;
using Temsa.Contracts.Artifacts;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models.AndroidLogcatCapture;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Android.Executors;

public class AndroidLogcatCaptureExecutor(
    IAndroidDeviceController deviceController,
    IArtifactStorage artifactStorage,
    IWorkerDeviceContext deviceContext,
    ILogger<AndroidLogcatCaptureExecutor> logger) : IAndroidLogcatCaptureExecutor
{
    private readonly IAndroidDeviceController _deviceController = deviceController;
    private readonly IArtifactStorage _artifactStorage = artifactStorage;
    private readonly IWorkerDeviceContext _deviceContext = deviceContext;
    private readonly ILogger<AndroidLogcatCaptureExecutor> _logger = logger;

    public async Task<AndroidLogcatCaptureExecutionResult> ExecuteAsync(
        AndroidLogcatCaptureTaskParameters parameters,
        WorkerTaskExecutionControl control,
        CancellationToken cancellationToken = default)
    {
        await control.Events.ReportProgressAsync(
            phase: "waiting_for_progress",
            message: $"Waiting for Android process '{parameters.PackageName}'",
            percent: 0,
            cancellationToken);

        var tempDirectory = CreateTempDirectory();
        var logPath = Path.Combine(tempDirectory, "logcat.txt");

        try
        {
            var processId = await WaitForProcessAsync(
                parameters.PackageName,
                TimeSpan.FromSeconds(parameters.ProcessWaitTimeoutSeconds),
                TimeSpan.FromMilliseconds(parameters.ProcessPollIntervalMilliseconds),
                control,
                cancellationToken);

            await control.Events.ReportProgressAsync(
                phase: "capturing_logcat",
                message: $"Capturing logcat for package '{parameters.PackageName}', pid {processId}",
                percent: 30,
                cancellationToken);

            var timeout = TimeSpan.FromSeconds(parameters.SessionTimeoutSeconds);

            var linesCount = await CaptureLogcatToFileAsync(
                processId,
                logPath,
                timeout,
                control,
                cancellationToken);

            await control.Events.ReportProgressAsync(
                phase: "uploading_logcat",
                message: "Uploading logcat artifacts",
                percent: 90,
                cancellationToken);

            await using var logStream = File.OpenRead(logPath);

            var objectKey = $"dynamic/android/{parameters.InputArtifact.Id}/{Guid.NewGuid():N}/logcat.txt";

            var storedLog = await _artifactStorage.UploadAsync(
                logStream,
                objectKey,
                fileName: "logcat.txt",
                contentType: "textp/plain",
                cancellationToken);

            var artifact = new ScanArtifactDescriptor(
                Kind: ArtifactKind.Log,
                Bucket: storedLog.Bucket,
                ObjectKey: storedLog.ObjectKey,
                FileName: storedLog.FileName,
                ContentType: storedLog.ContentType,
                SizeBytes: storedLog.SizeBytes);

            await control.Events.ReportProgressAsync(
                phase: "completed",
                message: "Android logcat capture completed",
                percent: 100,
                cancellationToken);

            return new AndroidLogcatCaptureExecutionResult(
                Status: "completed",
                PackageName: parameters.PackageName,
                ProcessId: processId,
                LinesCount: linesCount,
                Artifacts: [artifact]);
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    private async Task<int> WaitForProcessAsync(
        string packageName,
        TimeSpan timeout,
        TimeSpan pollInterval,
        WorkerTaskExecutionControl control,
        CancellationToken cancellationToken)
    {
        var deadline = DateTimeOffset.UtcNow.Add(timeout);
        var attempts = 0;

        while (DateTimeOffset.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (control.StopHandle.IsStopRequested)
            {
                throw new OperationCanceledException(
                    $"Stop requested while waiting for Android process '{packageName}'",
                    cancellationToken);
            }

            var processIds = await _deviceController.GetProcessIdsAsync(
                packageName,
                _deviceContext.DeviceId,
                cancellationToken);

            var processId = processIds.FirstOrDefault();

            if (processId > 0)
            {
                await control.Events.ReportProgressAsync(
                    phase: "process_found",
                    message: $"Android process '{packageName}' found. PID={processId}",
                    percent: 20,
                    cancellationToken);

                return processId;
            }

            attempts++;

            if (attempts % 10 == 0)
            {
                await control.Events.ReportProgressAsync(
                    phase: "waiting_for_process",
                    message: $"Waiting for Android process '{packageName}'",
                    percent: 10,
                    cancellationToken);
            }

            await Task.Delay(pollInterval, cancellationToken);
        }

        throw new InvalidOperationException(
            $"Android process for package '{packageName}' was not found within {timeout.TotalSeconds} seconds");
    }

    private async Task<long> CaptureLogcatToFileAsync(
        int processId,
        string logPath,
        TimeSpan timeout,
        WorkerTaskExecutionControl control,
        CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await using var writer = new StreamWriter(
            path: logPath,
            append: false,
            encoding: new UTF8Encoding(false));

        long linesCount = 0;

        var captureTask = _deviceController.CaptureLogcatAsync(
            processId,
            _deviceContext.DeviceId,
            async (line, token) =>
            {
                linesCount++;

                await writer.WriteLineAsync(line.AsMemory(), token);

                if (linesCount % 100 == 0)
                {
                    await writer.FlushAsync(token);
                }
            },
            linkedCts.Token);

        var stopTask = control.StopHandle.WaitForStopAsync(cancellationToken);
        var timeoutTask = Task.Delay(timeout, cancellationToken);

        var completedTask = await Task.WhenAny(captureTask, stopTask, timeoutTask);

        if (completedTask == stopTask)
        {
            await linkedCts.CancelAsync();

            await control.Events.ReportProgressAsync(
                phase: "interaction_completed",
                message: "Android logcat capture stop signal received",
                percent: 80,
                cancellationToken);
        }
        else if (completedTask == timeoutTask)
        {
            await linkedCts.CancelAsync();

            await control.Events.ReportProgressAsync(
                phase: "session_timeout",
                message: "Android logcat capture timeout reached",
                percent: 80,
                cancellationToken);
        }

        try
        {
            await captureTask;
        }
        catch (OperationCanceledException) when (
            cancellationToken.IsCancellationRequested ||
            control.StopHandle.IsStopRequested ||
            completedTask == timeoutTask) { }
        
        await writer.FlushAsync(cancellationToken);
        
        _logger.LogDebug(
            "Captured {LinesCount} logcat lines for pid {ProcessId}",
            linesCount,
            processId);

        return linesCount;
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            $"temsa-android-logcat-{Guid.NewGuid():N}");

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