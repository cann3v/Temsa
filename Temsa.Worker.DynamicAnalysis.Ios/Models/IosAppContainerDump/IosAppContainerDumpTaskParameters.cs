using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosAppContainerDump;

public record IosAppContainerDumpTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string BundleId,
    bool IncludeTmp = true,
    bool IncludeCaches = true);