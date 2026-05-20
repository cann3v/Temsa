using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidPrivateDataDump;

public record AndroidPrivateDataDumpExecutionResult(
    string Status,
    string PackageName,
    bool IncludeCache,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts);