namespace Temsa.Worker.StaticAnalysis.Models.Sast;

public record SastTaskParameters(
    long InputArtifactId,
    string Platform,
    string Tool,
    string? Ruleset,
    string? ReportFormat,
    int? Threads);