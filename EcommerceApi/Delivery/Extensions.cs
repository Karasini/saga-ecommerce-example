using Delivery.Consumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery;

public static class Extensions
{
    public static IServiceCollection AddDelivery(this IServiceCollection services)
    {
        return services;
    }

    public static IBusRegistrationConfigurator ConfigureDelivery(this
        IBusRegistrationConfigurator configure)
    {
        configure.AddConsumer<BookDeliveryRequestConsumer>();
        
        return configure;
    }
}