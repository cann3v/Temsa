namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosDynamicSession;

public record IosDynamicSessionPipelineParameters(
    string BundleId,
    int SessionTimeoutSeconds,
    string ScriptProfile,
    IReadOnlyCollection<string>? EnabledScripts,
    IReadOnlyCollection<string>? DisabledScripts);