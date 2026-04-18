using Microsoft.EntityFrameworkCore;
using Temsa.Core.Configuration;
using Temsa.Core.HealthChecks;
using Temsa.Core.Infrastructure.Messaging.RabbitMq;
using Temsa.Core.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Connection string 'Postgres' is not configured.");

builder.Services.AddDbContext<TemsaDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

builder.Services
    .AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("postgres")
    .AddCheck<RabbitMqHealthCheck>("rabbitmq");

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
