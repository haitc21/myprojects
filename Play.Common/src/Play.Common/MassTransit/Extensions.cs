using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit;
public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());

            configure.UsingRabbitMq((context, config) =>
            {
                var configuration = context.GetService<IConfiguration>();
                ServiceSettings serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var rabbitMqSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                config.Host(rabbitMqSettings.Host);
                config.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.Servicename, false));
                // https://masstransit-project.com/usage/exceptions.html#retry
                config.UseMessageRetry(retryConfig =>
                {
                    retryConfig.Interval(3, TimeSpan.FromSeconds(5));
                });
            });
        });

        // services.AddMassTransitHostedService(); // Version của MassTransit.RabbitMQ > 8 k cần 
        return services;

    }
}