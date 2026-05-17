namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidForceStopApp;

public record AndroidForceStopAppTaskParameters(
    string PackageName,
    string? DeviceId);