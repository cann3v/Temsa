namespace Temsa.Worker.DynamicAnalysis.Ios.Models;

public record IosDeviceOperationResult(
    string Status,
    string Operation,
    string? BundleId = null,
    string? DeviceId = null);