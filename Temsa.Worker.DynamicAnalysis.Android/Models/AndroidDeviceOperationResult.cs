namespace Temsa.Worker.DynamicAnalysis.Android.Models;

public record AndroidDeviceOperationResult(
    string Status,
    string Operation,
    string? PackageName = null,
    string? DeviceId = null);