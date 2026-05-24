using Temsa.Worker.StaticAnalysis.Radare2.Abstractions;
using Temsa.Worker.StaticAnalysis.Radare2.Context;
using Temsa.Worker.StaticAnalysis.Radare2.Models;

namespace Temsa.Worker.StaticAnalysis.Radare2.Analyzers;

public class IosAntiDebugAnalyzer : IRadare2Analyzer
{
    public string Id => "ios.antidebug";

    public async Task<IReadOnlyCollection<Radare2Finding>> AnalyzeAsync(
        IosBinaryContext context,
        CancellationToken cancellationToken = default)
    {
        var imports = await context.GetImportsAsync(cancellationToken);
        var strings = await context.GetStringsAsync(cancellationToken);

        var evidence = imports
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Where(x =>
                x!.Contains("ptrace", StringComparison.OrdinalIgnoreCase) ||
                x.Contains("sysctl", StringComparison.OrdinalIgnoreCase))
            .Concat(strings
                .Select(x => x.Value)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x =>
                    x!.Contains("PT_TRACE_ME", StringComparison.OrdinalIgnoreCase) ||
                    x.Contains("kinfo_proc", StringComparison.OrdinalIgnoreCase)))
            .Distinct()
            .ToArray();

        if (evidence.Length == 0)
        {
            return [];
        }

        return
        [
            new Radare2Finding(
                AnalyzerId: Id,
                TestId: "MASTG-TEST-0090",
                Status: Radare2FindingStatuses.ReviewItem,
                Message: "Found possible anti-debugging indicators",
                MasvsCategory: "MASVS-RESILIENCE",
                Severity: "medium",
                Confidence: "medium",
                Evidence: evidence)
        ];
    }
}