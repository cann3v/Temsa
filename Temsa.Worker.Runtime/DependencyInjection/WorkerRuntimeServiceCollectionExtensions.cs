using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Common.Time;
using Temsa.Worker.Runtime.Abstractions;
using Temsa.Worker.Runtime.Execution;
using Temsa.Worker.Runtime.Identity;
using Temsa.Worker.Runtime.Messaging.RabbitMq;

namespace Temsa.Worker.Runtime.DependencyInjection;

public static class WorkerRuntimeServiceCollectionExtensions
{
    public static IServiceCollection AddTemsaWorkerRuntime(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<RabbitMqConnectionOptions>()
            .Bind(configuration.GetSection($"{RabbitMqWorkerOptions.SectionName}:Connection"))
            .Validate(options =>
                    !string.IsNullOrWhiteSpace(options.Host) &&
                    !string.IsNullOrWhiteSpace(options.Username),
                "RabbitMQ worker connection configuration is invalid")
            .ValidateOnStart();

        services.AddOptions<RabbitMqWorkerEventsOptions>()
            .Bind(configuration.GetSection($"{RabbitMqWorkerOptions.SectionName}:Messaging"))
            .Validate(options =>
                    !string.IsNullOrWhiteSpace(options.QueueName) &&
                    !string.IsNullOrWhiteSpace(options.WorkerEventsExchange) &&
                    options.PrefetchCount > 0,
                "RabbitMQ worker messaging configuration is invalid")
            .ValidateOnStart();

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IWorkerIdentityProvider, DefaultWorkerIdentityProvider>();
        services.AddSingleton<IWorkerEventPublisher, RabbitMqWorkerEventPublisher>();
        services.AddSingleton<IWorkerTaskEventSinkFactory, WorkerTaskEventSinkFactory>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddHostedService<RabbitMqScanTaskConsumerHostedService>();

        return services;
    }
}