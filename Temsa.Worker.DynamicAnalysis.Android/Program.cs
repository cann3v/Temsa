using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Devices;
using Temsa.Worker.DynamicAnalysis.Android.Executors;
using Temsa.Worker.DynamicAnalysis.Android.Handlers;
using Temsa.Worker.DynamicAnalysis.Runtime.DependencyInjection;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTemsaWorkerRuntime(builder.Configuration);
builder.Services.AddTemsaDynamicAnalysisRuntime(builder.Configuration);

builder.Services.AddSingleton<IAndroidDeviceController, AdbAndroidDeviceController>();

builder.Services.AddSingleton<IWorkerTaskHandler, AndroidDynamicSessionTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, AndroidInstallAppTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, AndroidClearAppDataTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, AndroidUninstallAppTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, AndroidForceStopAppTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, AndroidLogcatCaptureTaskHandler>();

builder.Services.AddSingleton<IAndroidDynamicSessionExecutor, AndroidDynamicSessionExecutor>();
builder.Services.AddSingleton<IAndroidInstallAppExecutor, AndroidInstallAppExecutor>();
builder.Services.AddSingleton<IAndroidClearAppDataExecutor, AndroidClearAppDataExecutor>();
builder.Services.AddSingleton<IAndroidUninstallAppExecutor, AndroidUninstallAppExecutor>();
builder.Services.AddSingleton<IAndroidForceStopAppExecutor, AndroidForceStopAppExecutor>();
builder.Services.AddSingleton<IAndroidLogcatCaptureExecutor, AndroidLogcatCaptureExecutor>();


var host = builder.Build();
await host.RunAsync();