namespace Temsa.Worker.DynamicAnalysis.Runtime.Devices;

public interface IWorkerDeviceContext
{
    string Platform { get; }
    string DeviceId { get; }
}