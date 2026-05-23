using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosDynamicSession;

public record IosDynamicSessionTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string BundleId,
    int SessionTimeoutSeconds,
    string ScriptProfile,
    IReadOnlyCollection<string> EnabledScripts,
    IReadOnlyCollection<string> DisabledScripts);