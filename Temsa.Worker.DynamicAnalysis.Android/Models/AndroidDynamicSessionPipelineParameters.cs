namespace Temsa.Worker.DynamicAnalysis.Android.Models;

public record AndroidDynamicSessionPipelineParameters(
    string PackageName,
    string? DeviceId,
    int SessionTimeoutSeconds,
    string ScriptProfile,
    IReadOnlyCollection<string>? EnabledScripts,
    IReadOnlyCollection<string>? DisabledScripts);