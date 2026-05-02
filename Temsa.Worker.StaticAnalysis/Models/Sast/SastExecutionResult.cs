using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.StaticAnalysis.Models.Sast;

public record SastExecutionResult(
    string Tool,
    string Status,
    string? Ruleset,
    string? ReportFormat,
    int? Threads,
    int FindingsCount,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts);