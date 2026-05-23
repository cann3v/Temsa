using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Temsa.Common.Configuration;
using Temsa.Common.Files;
using Temsa.Common.Storage;
using Temsa.Contracts.Artifacts;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Models.Sast;

namespace Temsa.Worker.StaticAnalysis.Executors;

public class TruffleHogRadare2SastExecutor(
    IArtifactStorage artifactStorage,
    IOptions<StaticAnalysisToolOptions> options,
    ILogger<TruffleHogRadare2SastExecutor> logger) : ISastExecutor
{
    private readonly IArtifactStorage _artifactStorage = artifactStorage;
    private readonly StaticAnalysisToolOptions _options = options.Value;
    private readonly ILogger<TruffleHogRadare2SastExecutor> _logger = logger;

    public async Task<SastExecutionResult> ExecuteAsync(
        SastTaskParameters parameters,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken = default)
    {
        await using var tempDirectory = TemporaryDirectory.Create("temsa-ios-sast");

        var workspace = CreateWorkspace(tempDirectory.Path);

        await DownloadIpaAsync(parameters, workspace.IpaPath, events, cancellationToken);
        ExtractIpa(workspace.IpaPath, workspace.UnpackedDirectory);

        var appDirectory = FindAppBundleDirectory(workspace.UnpackedDirectory);
        var mainBinaryPath = FindMainBinaryPath(appDirectory);
        
        await events.ReportLogAsync(
            $"iOS app bundle detected: {Path.GetFileName(appDirectory)}",
            nameof(LogLevel.Information),
            cancellationToken);

        await events.ReportLogAsync(
            $"iOS main binary detected: {Path.GetFileName(mainBinaryPath)}",
            nameof(LogLevel.Information),
            cancellationToken);
        
        var truffleHogFindingsCount = await RunTruffleHogAsync(
            workspace.UnpackedDirectory,
            workspace.TruffleHogReportPath,
            events,
            cancellationToken);
        
        await RunRadare2Async(
            parameters,
            mainBinaryPath,
            workspace.Radare2ReportPath,
            events,
            cancellationToken);
        
        await events.ReportProgressAsync(
            phase: "uploading_reports",
            message: "Uploading trufflehog and r2 reports",
            percent: 90,
            cancellationToken);
        
        var artifacts = await UploadReportsAsync(
            parameters,
            workspace,
            cancellationToken);
        
        await events.ReportProgressAsync(
            phase: "completed",
            message: "iOS SAST execution completed",
            percent: 100,
            cancellationToken);

        await events.ReportLogAsync(
            $"iOS SAST completed. TruffleHog findings={truffleHogFindingsCount}",
            nameof(LogLevel.Information),
            cancellationToken);
        
        return new SastExecutionResult(
            Tool: parameters.Tool,
            Status: "completed",
            Ruleset: parameters.Ruleset,
            ReportFormat: "ndjson+text",
            Threads: parameters.Threads,
            FindingsCount: truffleHogFindingsCount,
            Artifacts: artifacts);
    }

    private static IosSastWorkspace CreateWorkspace(string tempDirectory)
    {
        return new IosSastWorkspace(
            IpaPath: Path.Combine(tempDirectory, "input.ipa"),
            UnpackedDirectory: Path.Combine(tempDirectory, "unpacked"),
            TruffleHogReportPath: Path.Combine(tempDirectory, "trufflehog-results.ndjson"),
            Radare2ReportPath: Path.Combine(tempDirectory, "radare2-report.txt"));
    }
    
    private async Task DownloadIpaAsync(
        SastTaskParameters parameters,
        string ipaPath,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken)
    {
        await events.ReportProgressAsync(
            phase: "downloading_input",
            message: "Downloading input IPA artifact",
            percent: 0,
            cancellationToken);

        await using var ipaStream = File.Create(ipaPath);

        await _artifactStorage.DownloadAsync(
            parameters.InputArtifact.Bucket,
            parameters.InputArtifact.ObjectKey,
            ipaStream,
            cancellationToken);
    }
    
    private static void ExtractIpa(
        string ipaPath,
        string unpackedDirectory)
    {
        Directory.CreateDirectory(unpackedDirectory);
        System.IO.Compression.ZipFile.ExtractToDirectory(ipaPath, unpackedDirectory);
    }
    
    private static string FindAppBundleDirectory(string unpackedDirectory)
    {
        var payloadDirectory = Path.Combine(unpackedDirectory, "Payload");

        if (!Directory.Exists(payloadDirectory))
        {
            throw new InvalidOperationException("IPA does not contain Payload directory");
        }

        var appDirectory = Directory
            .GetDirectories(payloadDirectory, "*.app", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();

        return appDirectory ??
               throw new InvalidOperationException("IPA does not contain .app bundle inside Payload directory");
    }
    
    private static string FindMainBinaryPath(string appDirectory)
    {
        var executableName = TryReadExecutableNameFromInfoPlist(appDirectory);

        if (!string.IsNullOrWhiteSpace(executableName))
        {
            var executablePath = Path.Combine(appDirectory, executableName);

            if (File.Exists(executablePath))
            {
                return executablePath;
            }
        }

        var appName = Path.GetFileNameWithoutExtension(appDirectory);
        var appNameBinaryPath = Path.Combine(appDirectory, appName);

        if (File.Exists(appNameBinaryPath))
        {
            return appNameBinaryPath;
        }

        var candidate = Directory
            .GetFiles(appDirectory, "*", SearchOption.TopDirectoryOnly)
            .Select(path => new FileInfo(path))
            .Where(file =>
                file is { Exists: true, Length: > 0 } &&
                string.IsNullOrWhiteSpace(file.Extension) &&
                !string.Equals(file.Name, "PkgInfo", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(file => file.Length)
            .FirstOrDefault();

        return candidate?.FullName
               ?? throw new InvalidOperationException(
                   $"Could not detect main iOS executable in app bundle '{appDirectory}'");
    }
    
    private static string? TryReadExecutableNameFromInfoPlist(string appDirectory)
    {
        var infoPlistPath = Path.Combine(appDirectory, "Info.plist");

        if (!File.Exists(infoPlistPath))
        {
            return null;
        }

        var content = File.ReadAllText(infoPlistPath);

        var match = Regex.Match(
            content,
            @"<key>\s*CFBundleExecutable\s*</key>\s*<string>\s*(?<value>[^<]+)\s*</string>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        return match.Success
            ? match.Groups["value"].Value.Trim()
            : null;
    }
    
    private async Task<int> RunTruffleHogAsync(
        string sourceDirectory,
        string reportPath,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken)
    {
        await events.ReportProgressAsync(
            phase: "running_trufflehog",
            message: "Running TruffleHog against extracted IPA",
            percent: 10,
            cancellationToken);

        var result = await RunProcessAsync(
            fileName: _options.TruffleHogExecutable,
            arguments:
            [
                "filesystem",
                sourceDirectory,
                "--json",
                "--no-update"
            ],
            workingDirectory: sourceDirectory,
            allowedExitCodes: [0],
            cancellationToken);

        await File.WriteAllTextAsync(
            reportPath,
            result.Stdout,
            cancellationToken);

        return CountNdjsonLines(reportPath);
    }
    
    private async Task RunRadare2Async(
        SastTaskParameters parameters,
        string binaryPath,
        string reportPath,
        IWorkerTaskEventSink events,
        CancellationToken cancellationToken)
    {
        await events.ReportProgressAsync(
            phase: "running_radare2",
            message: "Running radare2 scripts against iOS main binary",
            percent: 60,
            cancellationToken);

        var scripts = await ResolveRadare2ScriptsAsync(parameters, cancellationToken);

        if (scripts.Count == 0)
        {
            throw new InvalidOperationException("No radare2 scripts selected for execution");
        }

        var runnerScriptPath = Path.Combine(
            Path.GetDirectoryName(reportPath)!,
            "radare2-runner.r2");

        await BuildRadare2RunnerScriptAsync(
            scripts,
            runnerScriptPath,
            cancellationToken);

        await events.ReportLogAsync(
            $"Running radare2 analysis once with {scripts.Count} script(s): " +
            $"{string.Join(", ", scripts.Select(x => x.Id))}",
            nameof(LogLevel.Information),
            cancellationToken);

        var result = await RunProcessAsync(
            fileName: _options.Radare2Executable,
            arguments:
            [
                "-A",
                "-q",
                "-i",
                runnerScriptPath,
                binaryPath
            ],
            workingDirectory: Path.GetDirectoryName(binaryPath)!,
            allowedExitCodes: [0],
            cancellationToken);

        await File.WriteAllTextAsync(
            reportPath,
            result.Stdout,
            cancellationToken);
    }
    
    private async Task<IReadOnlyCollection<ScanArtifactDescriptor>> UploadReportsAsync(
        SastTaskParameters parameters,
        IosSastWorkspace workspace,
        CancellationToken cancellationToken)
    {
        var artifacts = new List<ScanArtifactDescriptor>
        {
            await UploadReportAsync(
                parameters,
                workspace.TruffleHogReportPath,
                fileName: "trufflehog-results.ndjson",
                contentType: "application/x-ndjson",
                cancellationToken),
            await UploadReportAsync(
                parameters,
                workspace.Radare2ReportPath,
                fileName: "radare2-report.txt",
                contentType: "text/plain",
                cancellationToken)
        };

        return artifacts;
    }
    
    private async Task<ScanArtifactDescriptor> UploadReportAsync(
        SastTaskParameters parameters,
        string reportPath,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(reportPath))
        {
            throw new InvalidOperationException($"Report file was not created: '{reportPath}'");
        }

        await using var reportStream = File.OpenRead(reportPath);

        var objectKey = $"static/ios/{parameters.InputArtifact.Id}/{Guid.NewGuid():N}/{fileName}";

        var storedReport = await _artifactStorage.UploadAsync(
            reportStream,
            objectKey,
            fileName,
            contentType,
            cancellationToken);

        return new ScanArtifactDescriptor(
            Kind: ArtifactKind.Report,
            Bucket: storedReport.Bucket,
            ObjectKey: storedReport.ObjectKey,
            FileName: storedReport.FileName,
            ContentType: storedReport.ContentType,
            SizeBytes: storedReport.SizeBytes);
    }
    
    private static int CountNdjsonLines(string reportPath)
    {
        if (!File.Exists(reportPath))
        {
            return 0;
        }

        return File
            .ReadLines(reportPath)
            .Count(line => !string.IsNullOrWhiteSpace(line));
    }
    
    private async Task<IReadOnlyCollection<ResolvedRadare2Script>> ResolveRadare2ScriptsAsync(
        SastTaskParameters parameters,
        CancellationToken cancellationToken)
    {
        var scriptsRoot = ResolveRadare2ScriptsRoot();
        var profileName = string.IsNullOrWhiteSpace(parameters.Radare2Profile)
            ? _options.DefaultRadare2Profile
            : parameters.Radare2Profile;

        var profilePath = Path.Combine(scriptsRoot, "profiles", $"{profileName}.json");

        if (!File.Exists(profilePath))
        {
            throw new FileNotFoundException(
                $"radare2 profile '{profileName}' was not found",
                profilePath);
        }

        await using var profileStream = File.OpenRead(profilePath);

        var profile = await System.Text.Json.JsonSerializer.DeserializeAsync<Radare2ScriptProfile>(
            profileStream,
            new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web),
            cancellationToken);

        if (profile is null)
        {
            throw new InvalidOperationException($"radare2 profile '{profileName}' is empty or invalid");
        }

        var enabledIds = ToHashSet(parameters.EnabledRadare2Scripts);
        var disabledIds = ToHashSet(parameters.DisabledRadare2Scripts);

        var selectedScripts = profile.Scripts
            .Where(script => ShouldRunRadare2Script(script, enabledIds, disabledIds))
            .Select(script => ResolveRadare2Script(scriptsRoot, script))
            .ToArray();

        var knownIds = profile.Scripts
            .Select(x => x.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        ValidateRequestedScripts(enabledIds, knownIds, profileName, "enabledRadare2Scripts");
        ValidateRequestedScripts(disabledIds, knownIds, profileName, "disabledRadare2Scripts");

        return selectedScripts;
    }
    
    private string ResolveRadare2ScriptsRoot()
    {
        var root = _options.Radare2ScriptsRoot;

        if (!Path.IsPathRooted(root))
        {
            root = Path.Combine(AppContext.BaseDirectory, root);
        }

        root = Path.GetFullPath(root);

        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException(
                $"radare2 scripts root was not found: '{root}'");
        }

        return root;
    }
    
    private static bool ShouldRunRadare2Script(
        Radare2ScriptDefinition script,
        IReadOnlySet<string> enabledIds,
        IReadOnlySet<string> disabledIds)
    {
        if (disabledIds.Contains(script.Id))
        {
            return false;
        }

        if (enabledIds.Count > 0)
        {
            return enabledIds.Contains(script.Id);
        }

        return script.Enabled;
    }
    
    private static ResolvedRadare2Script ResolveRadare2Script(
        string scriptsRoot,
        Radare2ScriptDefinition script)
    {
        if (string.IsNullOrWhiteSpace(script.Id))
        {
            throw new InvalidOperationException("radare2 script id is required");
        }

        if (string.IsNullOrWhiteSpace(script.Path))
        {
            throw new InvalidOperationException(
                $"radare2 script '{script.Id}' path is required");
        }

        var fullPath = Path.GetFullPath(Path.Combine(scriptsRoot, script.Path));

        if (!fullPath.StartsWith(scriptsRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"radare2 script '{script.Id}' path escapes scripts root");
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                $"radare2 script '{script.Id}' was not found",
                fullPath);
        }

        return new ResolvedRadare2Script(
            Id: script.Id,
            RelativePath: script.Path,
            FullPath: fullPath);
    }
    
    
    private static HashSet<string> ToHashSet(IReadOnlyCollection<string>? values)
    {
        return values?
                   .Where(x => !string.IsNullOrWhiteSpace(x))
                   .Select(x => x.Trim())
                   .ToHashSet(StringComparer.OrdinalIgnoreCase)
               ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
    
    private static void ValidateRequestedScripts(
        IReadOnlySet<string> requestedIds,
        IReadOnlySet<string> knownIds,
        string profileName,
        string parameterName)
    {
        var unknownIds = requestedIds
            .Where(id => !knownIds.Contains(id))
            .ToArray();

        if (unknownIds.Length == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Unknown radare2 script ids in '{parameterName}' for profile '{profileName}': {string.Join(", ", unknownIds)}");
    }
    
    private static async Task BuildRadare2RunnerScriptAsync(
        IReadOnlyCollection<ResolvedRadare2Script> scripts,
        string runnerScriptPath,
        CancellationToken cancellationToken)
    {
        await using var stream = File.Create(runnerScriptPath);
        await using var writer = new StreamWriter(stream);

        await writer.WriteLineAsync("e scr.color=false");
        await writer.WriteLineAsync("e scr.utf8=true");
        await writer.WriteLineAsync("echo # Temsa radare2 report");
        await writer.WriteLineAsync();

        foreach (var script in scripts)
        {
            var scriptContent = await File.ReadAllTextAsync(
                script.FullPath,
                cancellationToken);

            await writer.WriteLineAsync($"echo ## script: {script.Id}");
            await writer.WriteLineAsync($"echo ## path: {script.RelativePath}");
            await writer.WriteLineAsync(scriptContent.Trim());
            await writer.WriteLineAsync();
        }

        await writer.WriteLineAsync("q");
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

        using var process = new Process();
        process.StartInfo = startInfo;

        _logger.LogDebug(
            "Starting process: {FileName} {Arguments}",
            fileName,
            string.Join(" ", arguments));

        process.Start();

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (!allowedExitCodes.Contains(process.ExitCode))
        {
            throw new InvalidOperationException(
                $"Process '{fileName}' failed with exit code {process.ExitCode}. " +
                $"Stdout: {stdout} Stderr: {stderr}");
        }

        return new ProcessExecutionResult(
            ExitCode: process.ExitCode,
            Stdout: stdout,
            Stderr: stderr);
    }
    
    private record IosSastWorkspace(
        string IpaPath,
        string UnpackedDirectory,
        string TruffleHogReportPath,
        string Radare2ReportPath);
    
    private record ProcessExecutionResult(
        int ExitCode,
        string Stdout,
        string Stderr);
    
    private record Radare2ScriptProfile(
        string Name,
        IReadOnlyCollection<Radare2ScriptDefinition> Scripts);

    private record Radare2ScriptDefinition(
        string Id,
        string Path,
        bool Enabled);

    private record ResolvedRadare2Script(
        string Id,
        string RelativePath,
        string FullPath);

}