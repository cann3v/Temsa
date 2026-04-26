using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.DependencyInjection;
using Temsa.Worker.StaticAnalysis.Handlers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTemsaWorkerRuntime(builder.Configuration);

builder.Services.AddSingleton<IWorkerTaskHandler, DecompileTaskHandler>();
builder.Services.AddSingleton<IWorkerTaskHandler, SastTaskHandler>();

var host = builder.Build();
await host.RunAsync();