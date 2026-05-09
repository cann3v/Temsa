using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.StaticAnalysis.Models.Decompile;

public record DecompileExecutionResult(
    string Tool,
    string Status,
    string? OutputFormat,
    IReadOnlyCollection<ScanArtifactDescriptor> GeneratedArtifacts,
    string? Log);