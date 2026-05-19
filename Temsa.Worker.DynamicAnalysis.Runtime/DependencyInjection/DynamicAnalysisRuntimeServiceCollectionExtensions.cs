using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Temsa.Common.Configuration;
using Temsa.Common.Time;
using Temsa.Worker.DynamicAnalysis.Runtime.Devices;
using Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings;
using Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings.ClrBindings;
using Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings.Fakes;
using Temsa.Worker.DynamicAnalysis.Runtime.Scripts;

namespace Temsa.Worker.DynamicAnalysis.Runtime.DependencyInjection;

public static class DynamicAnalysisRuntimeServiceCollectionExtensions
{
    public static IServiceCollection AddTemsaDynamicAnalysisRuntime(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FridaScriptProviderOptions>(
            configuration.GetSection(FridaScriptProviderOptions.SectionName));
        services.Configure<DynamicWorkerDeviceOptions>(
            configuration.GetSection(DynamicWorkerDeviceOptions.SectionName));

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IFridaScriptProvider, FileFridaScriptProvider>();
        // services.AddSingleton<IFridaClient, FakeFridaClient>();
        services.AddSingleton<IFridaClient, FridaClrClient>();
        services.AddSingleton<IWorkerDeviceContext, WorkerDeviceContext>();
        
        return services;
    }
}