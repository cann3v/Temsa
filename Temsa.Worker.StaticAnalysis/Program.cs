using Temsa.Common.Configuration;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.DependencyInjection;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Executors;
using Temsa.Worker.StaticAnalysis.Handlers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTemsaWorkerRuntime(builder.Configuration);

builder.Services.AddOptions<StaticAnalysisToolOptions>()
    .Bind(builder.Configuration.GetSection(StaticAnalysisToolOptions.SectionName))
    .Validate(options => options.IsValid(), "Static analysis tool configuration is invalid")
    .ValidateOnStart();

builder.Services.AddSingleton<IWorkerTaskHandler, DecompileTaskHandler>();
builder.Services.AddSingleton<IDecompileExecutor, FakeDecompileExecutor>();
builder.Services.AddSingleton<IWorkerTaskHandler, SastTaskHandler>();
// builder.Services.AddSingleton<ISastExecutor, FakeSastExecutor>();
builder.Services.AddSingleton<ISastExecutor, JadxSemgrepSastExecutor>();

var host = builder.Build();
await host.RunAsync();