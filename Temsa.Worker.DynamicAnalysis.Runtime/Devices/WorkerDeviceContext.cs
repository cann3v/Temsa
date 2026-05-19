using Microsoft.Extensions.Options;
using Temsa.Common.Configuration;

namespace Temsa.Worker.DynamicAnalysis.Runtime.Devices;

public class WorkerDeviceContext(
    IOptions<DynamicWorkerDeviceOptions> options) : IWorkerDeviceContext
{
    private readonly DynamicWorkerDeviceOptions _options = options.Value;
    
    public string Platform => _options.Platform;
    public string DeviceId => _options.DeviceId;
}