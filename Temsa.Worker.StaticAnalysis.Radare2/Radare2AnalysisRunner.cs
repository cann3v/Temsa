using System.Text.Json;
using Microsoft.Extensions.Logging;
using Temsa.Worker.StaticAnalysis.Radare2.Abstractions;
using Temsa.Worker.StaticAnalysis.Radare2.Context;
using Temsa.Worker.StaticAnalysis.Radare2.Models;
using Temsa.Worker.StaticAnalysis.Radare2.Sessions;

namespace Temsa.Worker.StaticAnalysis.Radare2;

public class Radare2AnalysisRunner(
    IEnumerable<IRadare2Analyzer> analyzers,
    ILoggerFactory loggerFactory,
    ILogger<Radare2AnalysisRunner> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IReadOnlyCollection<IRadare2Analyzer> _analyzers = analyzers.ToArray();
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly ILogger<Radare2AnalysisRunner> _logger = logger;

    public async Task<int> AnalyzeAsync(
        string binaryPath,
        string findingsPath,
        string? radare2Executable,
        IReadOnlyCollection<string>? enabledAnalyzerIds,
        IReadOnlyCollection<string>? disabledAnalyzerIds,
        CancellationToken cancellationToken = default)
    {
        var selectedAnalyzers = ResolveAnalyzers(enabledAnalyzerIds, disabledAnalyzerIds);

        var resolvedRadare2Executable = string.IsNullOrWhiteSpace(radare2Executable)
            ? "radare2"
            : radare2Executable;

        await using IRadare2Session session = new Radare2PipeSession(resolvedRadare2Executable);

        await session.OpenAsync(binaryPath, cancellationToken);

        var context = new IosBinaryContext(session);

        await using var stream = File.Create(findingsPath);
        await using var writer = new StreamWriter(stream);

        var count = 0;

        foreach (var analyzer in selectedAnalyzers)
        {
            try
            {
                _logger.LogInformation("Running radare2 analyzer {AnalyzerId}", analyzer.Id);

                var findings = await analyzer.AnalyzeAsync(context, cancellationToken);

                foreach (var finding in findings)
                {
                    await writer.WriteLineAsync(JsonSerializer.Serialize(finding, JsonOptions));

                    if (!string.Equals(finding.Status, Radare2FindingStatuses.NotApplicable, StringComparison.OrdinalIgnoreCase))
                    {
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "radare2 analyzer {AnalyzerId} failed", analyzer.Id);

                var errorFinding = new Radare2Finding(
                    AnalyzerId: analyzer.Id,
                    TestId: "unknown",
                    Status: Radare2FindingStatuses.Error,
                    Message: ex.Message,
                    MasvsCategory: null,
                    Severity: null,
                    Confidence: null,
                    Evidence: []);

                await writer.WriteLineAsync(JsonSerializer.Serialize(errorFinding, JsonOptions));
                count++;
            }
        }

        return count;
    }
    
    private IReadOnlyCollection<IRadare2Analyzer> ResolveAnalyzers(
        IReadOnlyCollection<string>? enabledAnalyzerIds,
        IReadOnlyCollection<string>? disabledAnalyzerIds)
    {
        var enabled = ToHashSet(enabledAnalyzerIds);
        var disabled = ToHashSet(disabledAnalyzerIds);

        return _analyzers
            .Where(x => !disabled.Contains(x.Id))
            .Where(x => enabled.Count == 0 || enabled.Contains(x.Id))
            .ToArray();
    }
    
    private static HashSet<string> ToHashSet(IReadOnlyCollection<string>? values)
    {
        return values?
                   .Where(x => !string.IsNullOrWhiteSpace(x))
                   .Select(x => x.Trim())
                   .ToHashSet(StringComparer.OrdinalIgnoreCase)
               ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}