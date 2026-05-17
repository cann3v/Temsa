using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidInstallApp;

public record AndroidInstallAppTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string? DeviceId,
    bool Reinstall = true);