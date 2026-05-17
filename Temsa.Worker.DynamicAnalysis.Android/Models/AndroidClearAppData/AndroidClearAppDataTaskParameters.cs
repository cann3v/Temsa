namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidClearAppData;

public record AndroidClearAppDataTaskParameters(
    string PackageName,
    string? DeviceId);