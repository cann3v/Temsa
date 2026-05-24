using System.Text.RegularExpressions;
using Temsa.Worker.StaticAnalysis.Radare2.Abstractions;
using Temsa.Worker.StaticAnalysis.Radare2.Context;
using Temsa.Worker.StaticAnalysis.Radare2.Models;

namespace Temsa.Worker.StaticAnalysis.Radare2.Analyzers;

public class IosCryptoAnalyzer : IRadare2Analyzer
{
    private static readonly Regex LongHexRegex = new(@"\b[0-9A-Fa-f]{32,}\b", RegexOptions.Compiled);

    public string Id => "ios.crypto";
    
        public async Task<IReadOnlyCollection<Radare2Finding>> AnalyzeAsync(
        IosBinaryContext context,
        CancellationToken cancellationToken = default)
    {
        var findings = new List<Radare2Finding>();

        var imports = await context.GetImportsAsync(cancellationToken);
        var strings = await context.GetStringsAsync(cancellationToken);

        AddWeakCryptoImportFindings(imports, findings);
        AddHardcodedKeyHeuristics(strings, findings);
        await AddCCCryptXrefFindingsAsync(context, imports, findings, cancellationToken);

        return findings;
    }

    private void AddWeakCryptoImportFindings(
        IReadOnlyCollection<Radare2Import> imports,
        List<Radare2Finding> findings)
    {
        var weakImports = imports
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Where(x =>
                x!.Contains("CC_MD5", StringComparison.OrdinalIgnoreCase) ||
                x.Contains("CC_SHA1", StringComparison.OrdinalIgnoreCase) ||
                x.Contains("DES", StringComparison.OrdinalIgnoreCase) ||
                x.Contains("RC4", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToArray();

        if (weakImports.Length == 0)
        {
            return;
        }

        findings.Add(new Radare2Finding(
            AnalyzerId: Id,
            TestId: "MASTG-TEST-0213",
            Status: Radare2FindingStatuses.ReviewItem,
            Message: "Found references to weak or legacy cryptographic primitives",
            MasvsCategory: "MASVS-CRYPTO",
            Severity: "medium",
            Confidence: "medium",
            Evidence: weakImports));
    }

    private void AddHardcodedKeyHeuristics(
        IReadOnlyCollection<Radare2String> strings,
        List<Radare2Finding> findings)
    {
        var suspiciousStrings = strings
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Where(x => LongHexRegex.IsMatch(x!))
            .Distinct()
            .Take(10)
            .ToArray();

        if (suspiciousStrings.Length == 0)
        {
            return;
        }

        findings.Add(new Radare2Finding(
            AnalyzerId: Id,
            TestId: "MASTG-TEST-0214",
            Status: Radare2FindingStatuses.Heuristic,
            Message: "Found long hexadecimal strings that may represent hardcoded keys or secrets",
            MasvsCategory: "MASVS-CRYPTO",
            Severity: "medium",
            Confidence: "low",
            Evidence: suspiciousStrings));
    }

    private async Task AddCCCryptXrefFindingsAsync(
        IosBinaryContext context,
        IReadOnlyCollection<Radare2Import> imports,
        List<Radare2Finding> findings,
        CancellationToken cancellationToken)
    {
        var ccCryptImport = imports.FirstOrDefault(x =>
            !string.IsNullOrWhiteSpace(x.Name) &&
            x.Name.Contains("CCCrypt", StringComparison.OrdinalIgnoreCase));

        if (ccCryptImport?.Name is null)
        {
            return;
        }

        var xrefs = await context.Session.CmdJsonAsync<IReadOnlyCollection<Radare2Xref>>(
            $"axtj @ {ccCryptImport.Name}",
            cancellationToken) ?? [];

        var evidence = new List<string>
        {
            $"import={ccCryptImport.Name}",
            $"xrefs={xrefs.Count}"
        };

        foreach (var xref in xrefs.Take(5))
        {
            var instructions = await context.Session.CmdJsonAsync<IReadOnlyCollection<Radare2Instruction>>(
                $"pdj 12 @ {xref.From}",
                cancellationToken) ?? [];

            evidence.Add($"xref_from=0x{xref.From:x}");

            foreach (var instruction in instructions.Take(5))
            {
                if (!string.IsNullOrWhiteSpace(instruction.Disasm ?? instruction.Opcode))
                {
                    evidence.Add($"{instruction.Offset:x}: {instruction.Disasm ?? instruction.Opcode}");
                }
            }
        }

        findings.Add(new Radare2Finding(
            AnalyzerId: Id,
            TestId: "MASTG-TEST-0213",
            Status: Radare2FindingStatuses.ReviewItem,
            Message: "Found CCCrypt usage. Review algorithm, mode, padding, key and IV handling",
            MasvsCategory: "MASVS-CRYPTO",
            Severity: "medium",
            Confidence: "medium",
            Evidence: evidence));
    }
}