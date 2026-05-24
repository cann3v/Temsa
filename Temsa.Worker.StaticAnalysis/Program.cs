using Temsa.Common.Configuration;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.DependencyInjection;
using Temsa.Worker.StaticAnalysis.Abstractions;
using Temsa.Worker.StaticAnalysis.Executors;
using Temsa.Worker.StaticAnalysis.Handlers;
using Temsa.Worker.StaticAnalysis.Radare2;
using Temsa.Worker.StaticAnalysis.Radare2.Abstractions;
using Temsa.Worker.StaticAnalysis.Radare2.Analyzers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTemsaWorkerRuntime(builder.Configuration);

builder.Services.AddOptions<StaticAnalysisToolOptions>()
    .Bind(builder.Configuration.GetSection(StaticAnalysisToolOptions.SectionName))
    .Validate(options => options.IsValid(), "Static analysis tool configuration is invalid")
    .ValidateOnStart();

builder.Services.AddSingleton<IWorkerTaskHandler, DecompileTaskHandler>();
builder.Services.AddSingleton<IDecompileExecutor, FakeDecompileExecutor>();
builder.Services.AddSingleton<IWorkerTaskHandler, SastTaskHandler>();

builder.Services.AddSingleton<JadxSemgrepSastExecutor>();
builder.Services.AddSingleton<TruffleHogRadare2SastExecutor>();
builder.Services.AddSingleton<ISastExecutor, CompositeSastExecutor>();

builder.Services.AddSingleton<IRadare2Analyzer, IosAtsAnalyzer>();
builder.Services.AddSingleton<IRadare2Analyzer, IosCryptoAnalyzer>();
builder.Services.AddSingleton<IRadare2Analyzer, IosAntiDebugAnalyzer>();
builder.Services.AddSingleton<Radare2AnalysisRunner>();

var host = builder.Build();
await host.RunAsync();