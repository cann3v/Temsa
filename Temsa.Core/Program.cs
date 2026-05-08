using Microsoft.EntityFrameworkCore;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Common.Storage;
using Temsa.Common.Time;
using Temsa.Core.Application.Projects.Commands.CreateProject;
using Temsa.Core.Application.Projects.Commands.UploadProjectArtifact;
using Temsa.Core.Application.Projects.Queries.GetProject;
using Temsa.Core.Application.Projects.Queries.ListProjects;
using Temsa.Core.Application.Scans.Abstractions;
using Temsa.Core.Application.Scans.Commands.CreateScan;
using Temsa.Core.Application.Scans.Commands.StartScan;
using Temsa.Core.Application.Scans.Queries.GetScan;
using Temsa.Core.Application.Scans.Queries.ListScanTaskEvents;
using Temsa.Core.Application.Scans.Services;
using Temsa.Core.Application.WorkerEvents.Commands.HandleWorkerEvent;
using Temsa.Core.Configuration;
using Temsa.Core.HealthChecks;
using Temsa.Core.Infrastructure.Messaging.RabbitMq;
using Temsa.Core.Infrastructure.Persistence;
using Temsa.Core.Infrastructure.Pipelines.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.Configure<RabbitMqConnectionOptions>(
    builder.Configuration.GetSection($"{RabbitMqOptions.SectionName}:Connection"));
builder.Services.Configure<RabbitMqScanTasksOptions>(
    builder.Configuration.GetSection($"{RabbitMqOptions.SectionName}:Producer"));
builder.Services.Configure<RabbitMqWorkerEventsOptions>(
    builder.Configuration.GetSection($"{RabbitMqOptions.SectionName}:Consumer"));
builder.Services.Configure<JsonScanPipelineOptions>(
    builder.Configuration.GetSection(JsonScanPipelineOptions.SectionName));

builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddSingleton<IScanPipelineProvider, JsonScanPipelineProvider>();
builder.Services.AddSingleton<ScanStatusCalculator>();
builder.Services.AddSingleton<IArtifactStorage, FakeArtifactStorage>();

builder.Services.AddScoped<CreateProjectHandler>();
builder.Services.AddScoped<GetProjectHandler>();
builder.Services.AddScoped<ListProjectsHandler>();
builder.Services.AddScoped<CreateScanHandler>();
builder.Services.AddScoped<GetScanHandler>();
builder.Services.AddScoped<IScanTaskPublisher, RabbitMqScanTaskPublisher>();
builder.Services.AddScoped<StartScanHandler>();
builder.Services.AddScoped<HandleWorkerEventHandler>();
builder.Services.AddScoped<ListScanTaskEventsHandler>();
builder.Services.AddScoped<UploadProjectArtifactHandler>();

builder.Services.AddHostedService<RabbitMqWorkerEventsHostedService>();

var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Connection string 'Postgres' is not configured.");

builder.Services.AddDbContext<TemsaDbContext>(options => options.UseNpgsql(postgresConnectionString));

builder.Services
    .AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("postgres")
    .AddCheck<RabbitMqHealthCheck>("rabbitmq");

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MyAPI");
    });
}

app.UseHttpsRedirection();

app.Run();
