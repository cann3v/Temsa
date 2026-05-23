using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosInstallApp;

public record IosInstallAppTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string? BundleId);