namespace Temsa.Worker.StaticAnalysis.Radare2.Models;

public record Radare2Finding(
    string AnalyzerId,
    string TestId,
    string Status,
    string Message,
    string? MasvsCategory,
    string? Severity,
    string? Confidence,
    IReadOnlyCollection<string> Evidence);