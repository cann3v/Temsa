namespace Temsa.Common.Configuration;

public class StaticAnalysisToolOptions
{
    public const string SectionName = "StaticAnalysisTools";

    public string JadxExecutable { get; init; } = "jadx";
    public string SemgrepExecutable { get; init; } = "semgrep";
    public string SemgrepRulesPath { get; init; } = string.Empty;

    public string TruffleHogExecutable { get; init; } = "trufflehog";
    public string Radare2Executable { get; init; } = "r2";
    public string Radare2ScriptsRoot { get; init; } = "Scripts/r2";
    public string DefaultRadare2Profile { get; init; } = "ios-basic";
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(JadxExecutable) &&
               !string.IsNullOrWhiteSpace(SemgrepExecutable) &&
               !string.IsNullOrWhiteSpace(SemgrepRulesPath) &&
               Directory.Exists(SemgrepRulesPath) &&
               !string.IsNullOrWhiteSpace(TruffleHogExecutable) &&
               !string.IsNullOrWhiteSpace(Radare2Executable) &&
               !string.IsNullOrWhiteSpace(Radare2ScriptsRoot) &&
               !string.IsNullOrWhiteSpace(DefaultRadare2Profile);
    }
}