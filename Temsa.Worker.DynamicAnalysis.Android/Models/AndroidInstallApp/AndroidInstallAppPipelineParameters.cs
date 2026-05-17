namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidInstallApp;

public record AndroidInstallAppPipelineParameters(
    string? DeviceId,
    bool Reinstall = true);