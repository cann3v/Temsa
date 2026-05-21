using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Temsa.Common.Configuration;
using Temsa.Common.Files;
using Temsa.Common.Storage;
using Temsa.Contracts.Artifacts;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Sast;

namespace Temsa.Worker.StaticAnalysis.Executors;

public class JadxSemgrepSastExecutor(
    IArtifactStorage artifactStorage,
    IOptions<StaticAnalysisToolOptions> options,
    ILogger<JadxSemgrepSastExecutor> logger) : ISastExecutor
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IArtifactStorage _artifactStorage = artifactStorage;
    private readonly StaticAnalysisToolOptions _options = options.Value;
    private readonly ILogger<JadxSemgrepSastExecutor> _logger = logger;

    public async Task<SastExecutionResult> ExecuteAsync(
        SastTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await using var tempDirectory = TemporaryDirectory.Create("temsa-sast");

        var apkPath = Path.Combine(tempDirectory.Path, "input.apk");
        var decompiledDirectory = Path.Combine(tempDirectory.Path, "decompiled");
        var reportPath = Path.Combine(tempDirectory.Path, "semgrep-report.sarif");
        
        await events.ReportProgressAsync(
            phase: "downloading_input",
            message: "Downloading input APK artifact",
            percent: 0,
            cancellationToken);
        
        await using (var apkStream = File.Create(apkPath))
        {
            await _artifactStorage.DownloadAsync(
                parameters.InputArtifact.Bucket,
                parameters.InputArtifact.ObjectKey,
                apkStream,
                cancellationToken);
        }
        
        Directory.CreateDirectory(decompiledDirectory);
        
        await events.ReportProgressAsync(
            phase: "decompiling",
            message: "Decompiling APK with JADX",
            percent: 20,
            cancellationToken);
        
        var jadxResult = await RunProcessAsync(
            fileName: _options.JadxExecutable,
            arguments:
            [
                "-d",
                decompiledDirectory,
                apkPath
            ],
            workingDirectory: tempDirectory.Path,
            allowedExitCodes: [0, 3],
            cancellationToken);
        
        if (jadxResult.ExitCode != 0)
        {
            await events.ReportLogAsync(
                message: $"JADX completed with non-zero exit code {jadxResult.ExitCode}, but usable output was produced",
                level: nameof(LogLevel.Warning),
                cancellationToken);
        }
        
        await events.ReportProgressAsync(
            phase: "running_semgrep",
            message: "Running Semgrep Android security rules",
            percent: 50,
            cancellationToken);
        
        var semgrepArguments = new List<string>
        {
            "scan",
            "--config",
            _options.SemgrepRulesPath,
            "--sarif",
            "--output",
            reportPath,
            decompiledDirectory
        };
        
        if (parameters.Threads is > 0)
        {
            semgrepArguments.InsertRange(1, ["--jobs", parameters.Threads.Value.ToString()]);
        }
        
        await RunProcessAsync(
            fileName: _options.SemgrepExecutable,
            arguments: semgrepArguments,
            workingDirectory: tempDirectory.Path,
            allowedExitCodes: [0],
            cancellationToken);
        
        if (!File.Exists(reportPath))
        {
            throw new InvalidOperationException(
                $"Semgrep report was not created at '{reportPath}'");
        }
        
        var findingsCount = await CountSemgrepFindingsAsync(
            reportPath,
            cancellationToken);
        
        await events.ReportProgressAsync(
            phase: "uploading_report",
            message: $"Uploading Semgrep report. Findings={findingsCount}",
            percent: 90,
            cancellationToken);
        
        await using var reportStream = File.OpenRead(reportPath);

        var objectKey = $"static/android/{parameters.InputArtifact.Id}/{Guid.NewGuid():N}/semgrep-report.sarif";

        var storedReport = await _artifactStorage.UploadAsync(
            reportStream,
            objectKey,
            fileName: "semgrep-report.sarif",
            contentType: "application/sarif+json",
            cancellationToken);
        
        var artifact = new ScanArtifactDescriptor(
            Kind: ArtifactKind.Report,
            Bucket: storedReport.Bucket,
            ObjectKey: storedReport.ObjectKey,
            FileName: storedReport.FileName,
            ContentType: storedReport.ContentType,
            SizeBytes: storedReport.SizeBytes);
        
        await events.ReportLogAsync(
            message: $"Semgrep completed. Findings={findingsCount}",
            level: nameof(LogLevel.Information),
            cancellationToken);

        await events.ReportProgressAsync(
            phase: "completed",
            message: "SAST execution completed",
            percent: 100,
            cancellationToken);
        
        return new SastExecutionResult(
            Tool: parameters.Tool,
            Status: "completed",
            Ruleset: parameters.Ruleset,
            ReportFormat: "sarif",
            Threads: parameters.Threads,
            FindingsCount: findingsCount,
            Artifacts: [artifact]);
    }
    
    private static async Task<int> CountSemgrepFindingsAsync(
        string reportPath,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(reportPath);

        using var document = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("runs", out var runs) ||
            runs.ValueKind != JsonValueKind.Array)
        {
            return 0;
        }

        var count = 0;

        foreach (var run in runs.EnumerateArray())
        {
            if (!run.TryGetProperty("results", out var results) ||
                results.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            count += results.GetArrayLength();
        }

        return count;
    }

    private async Task<ProcessExecutionResult> RunProcessAsync(
        string fileName,
        IReadOnlyCollection<string> arguments,
        string workingDirectory,
        IReadOnlyCollection<int> allowedExitCodes,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo
        };

        _logger.LogInformation(
            "Starting process: {FileName} {Arguments}",
            fileName,
            string.Join(" ", arguments));

        process.Start();

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (!string.IsNullOrWhiteSpace(stdout))
        {
            _logger.LogDebug(
                "Process {FileName} stdout: {Stdout}",
                fileName,
                stdout);
        }

        if (!string.IsNullOrWhiteSpace(stderr))
        {
            _logger.LogDebug(
                "Process {FileName} stderr: {Stderr}",
                fileName,
                stderr);
        }

        if (!allowedExitCodes.Contains(process.ExitCode))
        {
            throw new InvalidOperationException(
                $"Process '{fileName}' failed with exit code {process.ExitCode}. " +
                $"Stdout: {stdout} Stderr: {stderr}");
        }

        return new ProcessExecutionResult(
            process.ExitCode,
            stdout,
            stderr);
    }

    
    private record ProcessExecutionResult(
        int ExitCode,
        string Stdout,
        string Stderr);
}