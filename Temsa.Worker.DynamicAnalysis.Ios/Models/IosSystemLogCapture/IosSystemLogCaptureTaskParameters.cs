using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosSystemLogCapture;

public record IosSystemLogCaptureTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string? BundleId,
    int SessionTimeoutSeconds);