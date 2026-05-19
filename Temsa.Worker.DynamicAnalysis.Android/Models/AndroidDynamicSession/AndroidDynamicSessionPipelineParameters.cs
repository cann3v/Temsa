namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidDynamicSession;

public record AndroidDynamicSessionPipelineParameters(
    string PackageName,
    int SessionTimeoutSeconds,
    string ScriptProfile,
    IReadOnlyCollection<string>? EnabledScripts,
    IReadOnlyCollection<string>? DisabledScripts);