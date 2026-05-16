using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models;

public record AndroidDynamicSessionTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string PackageName,
    string? DeviceId,
    int SessionTimeoutSeconds,
    string ScriptProfile,
    IReadOnlyCollection<string> EnabledScripts,
    IReadOnlyCollection<string> DisabledScripts);