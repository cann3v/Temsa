using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosAppContainerDump;

public record IosAppContainerDumpExecutionResult(
    string Status,
    string? BundleId,
    bool IncludeTmp,
    bool IncludeCaches,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts);