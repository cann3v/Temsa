namespace Temsa.Common.Configuration;

public class DynamicWorkerDeviceOptions
{
    public const string SectionName = "DynamicWorkerDevice";

    public string Platform { get; init; } = string.Empty;
    public string DeviceId { get; init; } = string.Empty;
}