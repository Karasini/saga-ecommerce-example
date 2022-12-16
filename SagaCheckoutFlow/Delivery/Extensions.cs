using Delivery.Consumers;
using Delivery.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery;

public static class Extensions
{
    public static IServiceCollection AddDelivery(this IServiceCollection services)
    {
        services.AddScoped<IDeliveryService, DeliveryService>();
        return services;
    }

    public static IBusRegistrationConfigurator ConfigureDelivery(this
        IBusRegistrationConfigurator configure)
    {
        configure.AddConsumer<BookDeliveryRequestConsumer>();
        
        return configure;
    }
}