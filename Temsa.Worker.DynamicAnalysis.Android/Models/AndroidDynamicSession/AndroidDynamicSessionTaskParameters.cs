using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidDynamicSession;

public record AndroidDynamicSessionTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string PackageName,
    int SessionTimeoutSeconds,
    string ScriptProfile,
    IReadOnlyCollection<string> EnabledScripts,
    IReadOnlyCollection<string> DisabledScripts);