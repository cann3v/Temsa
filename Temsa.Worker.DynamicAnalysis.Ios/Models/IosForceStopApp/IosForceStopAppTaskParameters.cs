using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosForceStopApp;

public record IosForceStopAppTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string BundleId);