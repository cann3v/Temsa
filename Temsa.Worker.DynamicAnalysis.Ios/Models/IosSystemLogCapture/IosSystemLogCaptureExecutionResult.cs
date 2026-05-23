using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosSystemLogCapture;

public record IosSystemLogCaptureExecutionResult(
    string Status,
    string? BundleId,
    int LinesCount,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts);