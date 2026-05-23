using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosUninstallApp;

public record IosUninstallAppTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string BundleId);