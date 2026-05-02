namespace Temsa.Worker.StaticAnalysis.Models.Decompile;

public record DecompileTaskParameters(
    long InputArtifactId,
    string Platform,
    string Tool,
    string? OutputFormat);