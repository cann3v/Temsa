using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidLogcatCapture;

public record AndroidLogcatCaptureExecutionResult(
    string Status,
    string PackageName,
    int ProcessId,
    long LinesCount,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts);