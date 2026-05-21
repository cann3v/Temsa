namespace Temsa.Common.Configuration;

public class StaticAnalysisToolOptions
{
    public const string SectionName = "StaticAnalysisTools";

    public string JadxExecutable { get; init; } = "jadx";
    public string SemgrepExecutable { get; init; } = "semgrep";
    public string SemgrepRulesPath { get; init; } = string.Empty;
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(JadxExecutable) &&
               !string.IsNullOrWhiteSpace(SemgrepExecutable) &&
               !string.IsNullOrWhiteSpace(SemgrepRulesPath) &&
               Directory.Exists(SemgrepRulesPath);
    }
}