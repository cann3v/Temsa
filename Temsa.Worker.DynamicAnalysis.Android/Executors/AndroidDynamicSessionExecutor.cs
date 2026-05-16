using System.Text;
using System.Text.Json;
using Temsa.Common.Storage;
using Temsa.Contracts.Artifacts;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Models;
using Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings;
using Temsa.Worker.DynamicAnalysis.Runtime.Scripts;
using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.DynamicAnalysis.Android.Executors;

public class AndroidDynamicSessionExecutor(
    IArtifactStorage artifactStorage,
    IFridaScriptProvider scriptProvider,
    IFridaClient fridaClient,
    ILogger<AndroidDynamicSessionExecutor> logger) : IAndroidDynamicSessionExecutor
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    
    private readonly IArtifactStorage _artifactStorage = artifactStorage;
    private readonly IFridaScriptProvider _scriptProvider = scriptProvider;
    private readonly IFridaClient _fridaClient = fridaClient;
    private readonly ILogger<AndroidDynamicSessionExecutor> _logger = logger;

    public async Task<AndroidDynamicSessionExecutionResult> ExecuteAsync(
        AndroidDynamicSessionTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await events.ReportProgressAsync(
            phase: "preparing",
            message: "Preparing Android dynamic analysis session",
            percent: 0,
            cancellationToken);

        var tempDirectory = CreateTempDirectory();
        var logPath = Path.Combine(tempDirectory, "frida-events.ndjson");

        try
        {
            var inputFilePath = Path.Combine(
                tempDirectory,
                parameters.InputArtifact.FileName ?? "input.apk");

            await using (var inputFile = File.Create(inputFilePath))
            {
                await _artifactStorage.DownloadAsync(
                    parameters.InputArtifact.Bucket,
                    parameters.InputArtifact.ObjectKey,
                    inputFile,
                    cancellationToken);
            }

            await events.ReportProgressAsync(
                phase: "input_download",
                message: "Input APK downloaded from artifact storage",
                percent: 10,
                cancellationToken);

            var loadedScripts = await _scriptProvider.LoadScriptAsync(
                parameters.ScriptProfile,
                parameters.EnabledScripts,
                parameters.DisabledScripts,
                cancellationToken);

            await events.ReportProgressAsync(
                phase: "scripts_loaded",
                message: $"Loaded {loadedScripts.Count} Frida scripts from profile `{parameters.ScriptProfile}`",
                percent: 25,
                cancellationToken);

            var fridaScripts = loadedScripts
                .Select(x => new FridaScriptDefinition(
                    Id: x.Id,
                    Source: x.Source))
                .ToArray();

            await using var fridaSession = await _fridaClient.SpawnAsync(
                new FridaSpawnOptions(
                    TargetIdentifier: parameters.PackageName,
                    DeviceId: parameters.DeviceId,
                    Scripts: fridaScripts),
                cancellationToken);

            await events.ReportProgressAsync(
                phase: "waiting_for_user",
                message: "Android application is running with Frida scripts loaded",
                percent: 50,
                cancellationToken);

            var timeout = TimeSpan.FromSeconds(parameters.SessionTimeoutSeconds);
            var messagesCount = await CollectFridaMessagesAsync(
                fridaSession,
                logPath,
                timeout,
                events,
                cancellationToken);

            await fridaSession.DetachAsync(cancellationToken);

            await events.ReportProgressAsync(
                phase: "collecting_artifacts",
                message: "Uploading raw Frida log artifact",
                percent: 90,
                cancellationToken);

            await using var logStream = File.OpenRead(logPath);

            var objectKey = $"dynamic/android/{parameters.InputArtifact.Id}/{Guid.NewGuid():N}/frida-events.ndjson";

            var storedLog = await _artifactStorage.UploadAsync(
                logStream,
                objectKey,
                fileName: "frida-events.ndjson",
                contentType: "application/x-ndjson",
                cancellationToken);

            var artifact = new ScanArtifactDescriptor(
                Kind: ArtifactKind.Log,
                Bucket: storedLog.Bucket,
                ObjectKey: storedLog.ObjectKey,
                FileName: storedLog.FileName,
                ContentType: storedLog.ContentType,
                SizeBytes: storedLog.SizeBytes);

            await events.ReportProgressAsync(
                phase: "completed",
                message: "Android dynamic analysis session completed",
                percent: 100,
                cancellationToken);

            return new AndroidDynamicSessionExecutionResult(
                Status: "completed",
                PackageName: parameters.PackageName,
                FridaMessagesCount: messagesCount,
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
            $"temsa-android-dynamic-{Guid.NewGuid():N}");

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

    private static async Task<int> CollectFridaMessagesAsync(
        IFridaSession fridaSession,
        string logPath,
        TimeSpan timeout,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeoutCts.Token);

        var count = 0;

        await using var writer = new StreamWriter(
            path: logPath,
            append: false,
            encoding: new UTF8Encoding(false));

        try
        {
            await foreach (var message in fridaSession.ReadMessagesAsync(linkedCts.Token))
            {
                count++;

                var json = JsonSerializer.Serialize(message, JsonSerializerOptions);
                await writer.WriteLineAsync(json);
                await writer.FlushAsync(linkedCts.Token);

                if (count <= 5 || count % 10 == 0)
                {
                    await events.ReportLogAsync(
                        message: $"Frida message received: {message.ScriptId}",
                        level: nameof(LogLevel.Information),
                        linkedCts.Token);
                }
            }
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            await events.ReportProgressAsync(
                phase: "session_timeout",
                message: "Android dynamic session timeout reached",
                percent: 80,
                cancellationToken);
        }

        return count;
    }
}