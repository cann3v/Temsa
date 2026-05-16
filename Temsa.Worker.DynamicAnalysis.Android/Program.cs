using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Temsa.Worker.DynamicAnalysis.Android.Abstractions;
using Temsa.Worker.DynamicAnalysis.Android.Executors;
using Temsa.Worker.DynamicAnalysis.Android.Handlers;
using Temsa.Worker.DynamicAnalysis.Runtime.DependencyInjection;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTemsaWorkerRuntime(builder.Configuration);
builder.Services.AddTemsaDynamicAnalysisRuntime(builder.Configuration);

builder.Services.AddSingleton<IAndroidDynamicSessionExecutor, AndroidDynamicSessionExecutor>();
builder.Services.AddSingleton<IWorkerTaskHandler, AndroidDynamicSessionTaskHandler>();

var host = builder.Build();
await host.RunAsync();