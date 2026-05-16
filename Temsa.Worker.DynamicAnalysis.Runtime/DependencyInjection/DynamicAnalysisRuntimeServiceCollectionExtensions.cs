using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Temsa.Common.Configuration;
using Temsa.Worker.DynamicAnalysis.Runtime.Frida;
using Temsa.Worker.DynamicAnalysis.Runtime.Frida.Fakes;
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

        services.AddSingleton<IFridaScriptProvider, FileFridaScriptProvider>();
        services.AddSingleton<IFridaClient, FakeFridaClient>();

        return services;
    }
}