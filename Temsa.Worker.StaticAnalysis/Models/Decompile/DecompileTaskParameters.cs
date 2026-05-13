using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.StaticAnalysis.Models.Decompile;

public record DecompileTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string Platform,
    string Tool,
    string? OutputFormat);