using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidInstallApp;

public record AndroidInstallAppTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    bool Reinstall = true);