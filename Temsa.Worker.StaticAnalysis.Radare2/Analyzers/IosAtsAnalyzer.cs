using Temsa.Worker.StaticAnalysis.Radare2.Abstractions;
using Temsa.Worker.StaticAnalysis.Radare2.Context;
using Temsa.Worker.StaticAnalysis.Radare2.Models;

namespace Temsa.Worker.StaticAnalysis.Radare2.Analyzers;

public class IosAtsAnalyzer : IRadare2Analyzer
{
    public string Id => "ios.ats";

    public async Task<IReadOnlyCollection<Radare2Finding>> AnalyzeAsync(
        IosBinaryContext context,
        CancellationToken cancellationToken = default)
    {
        var strings = await context.GetStringsAsync(cancellationToken);

        var indicators = new[]
        {
            "NSAllowsArbitraryLoads",
            "NSExceptionAllowsInsecureHTTPLoads",
            "NSAppTransportSecurity",
            "http://"
        };

        var evidence = strings
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Where(x => indicators.Any(indicator =>
                x!.Contains(indicator, StringComparison.OrdinalIgnoreCase)))
            .Distinct()
            .Take(20)
            .ToArray();

        if (evidence.Length == 0)
        {
            return
            [
                new Radare2Finding(
                    AnalyzerId: Id,
                    TestId: "MASTG-TEST-0322",
                    Status: Radare2FindingStatuses.NotApplicable,
                    Message: "No ATS exceptions or cleartext HTTP indicators were found in binary strings",
                    MasvsCategory: "MASVS-NETWORK",
                    Severity: null,
                    Confidence: "medium",
                    Evidence: [])
            ];
        }

        return
        [
            new Radare2Finding(
                AnalyzerId: Id,
                TestId: "MASTG-TEST-0322",
                Status: Radare2FindingStatuses.Heuristic,
                Message: "Found ATS exception or cleartext HTTP indicators in binary strings",
                MasvsCategory: "MASVS-NETWORK",
                Severity: "medium",
                Confidence: "medium",
                Evidence: evidence)
        ];
    }
}