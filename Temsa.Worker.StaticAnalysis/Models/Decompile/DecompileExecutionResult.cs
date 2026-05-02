namespace Temsa.Worker.StaticAnalysis.Models.Decompile;

public record DecompileExecutionResult(
    string Tool,
    string Status,
    string? OutputFormat,
    IReadOnlyCollection<object> GeneratedArtifacts,
    string? Log);