using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.StaticAnalysis.Models.Sast;

public record SastTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string Platform,
    string Tool,
    string? Ruleset,
    string? ReportFormat,
    int? Threads,
    string? Radare2Profile,
    IReadOnlyCollection<string>? EnabledRadare2Scripts,
    IReadOnlyCollection<string>? DisabledRadare2Scripts);