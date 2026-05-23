using Temsa.Worker.DynamicAnalysis.Ios.Abstractions;
using Temsa.Worker.DynamicAnalysis.Ios.Executors;
using Temsa.Worker.DynamicAnalysis.Ios.Handlers;
using Temsa.Worker.DynamicAnalysis.Runtime.DependencyInjection;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddTemsaWorkerRuntime(builder.Configuration);
builder.Services.AddTemsaDynamicAnalysisRuntime(builder.Configuration);

builder.Services.AddSingleton<IWorkerTaskHandler, IosInstallAppTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, IosDynamicSessionTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, IosSystemLogCaptureTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, IosForceStopAppTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, IosAppContainerDumpTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, IosUninstallAppTaskHandler>();

builder.Services.AddSingleton<IIosInstallAppExecutor, IosInstallAppExecutor>();
builder.Services.AddSingleton<IIosDynamicSessionExecutor, IosDynamicSessionExecutor>();
builder.Services.AddSingleton<IIosSystemLogCaptureExecutor, IosSystemLogCaptureExecutor>();
builder.Services.AddSingleton<IIosForceStopAppExecutor, IosForceStopAppExecutor>();
builder.Services.AddSingleton<IIosAppContainerDumpExecutor, IosAppContainerDumpExecutor>();
builder.Services.AddSingleton<IIosUninstallAppExecutor, IosUninstallAppExecutor>();


var host = builder.Build();
await host.RunAsync();