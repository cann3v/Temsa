namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidUninstallApp;

public record AndroidUninstallAppTaskParameters(
    string PackageName,
    string? DeviceId);