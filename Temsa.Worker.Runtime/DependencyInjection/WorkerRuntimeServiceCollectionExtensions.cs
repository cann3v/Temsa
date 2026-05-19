using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Temsa.Common.Configuration;
using Temsa.Common.RabbitMq;
using Temsa.Common.Storage;
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
        
        services.AddOptions<RabbitMqWorkerControlOptions>()
            .Bind(configuration.GetSection($"{RabbitMqWorkerOptions.SectionName}:Control"))
            .Validate(options =>
                    !string.IsNullOrWhiteSpace(options.WorkerControlExchange) &&
                    !string.IsNullOrWhiteSpace(options.QueueName) &&
                    !string.IsNullOrWhiteSpace(options.RoutingKey),
                "RabbitMQ worker control configuration is invalid")
            .ValidateOnStart();

        services.AddOptions<ArtifactStorageOptions>()
            .Bind(configuration.GetSection(ArtifactStorageOptions.SectionName))
            .Validate(options =>
                    !string.IsNullOrWhiteSpace(options.Endpoint) &&
                    !string.IsNullOrWhiteSpace(options.AccessKey) &&
                    !string.IsNullOrWhiteSpace(options.SecretKey),
                "Artifact storage configuration is invalid")
            .ValidateOnStart();

        services.AddSingleton<IAmazonS3>(serviceProvider =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<ArtifactStorageOptions>>()
                .Value;

            var credentials = new BasicAWSCredentials(
                options.AccessKey,
                options.SecretKey);

            var config = new AmazonS3Config
            {
                ServiceURL = options.Endpoint,
                ForcePathStyle = options.ForcePathStyle,
                AuthenticationRegion = options.Region
            };

            return new AmazonS3Client(credentials, config);
        });

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IWorkerIdentityProvider, DefaultWorkerIdentityProvider>();
        services.AddSingleton<IWorkerEventPublisher, RabbitMqWorkerEventPublisher>();
        services.AddSingleton<IWorkerTaskEventSinkFactory, WorkerTaskEventSinkFactory>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IArtifactStorage, S3ArtifactStorage>();
        services.AddSingleton<IRunningWorkerTaskRegistry, RunningWorkerTaskRegistry>();
        services.AddHostedService<RabbitMqScanTaskConsumerHostedService>();
        services.AddHostedService<RabbitMqWorkerControlConsumerHostedService>();

        return services;
    }
}