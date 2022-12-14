using System;
using Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Consumers;

namespace Warehouse;

public static class Extensions
{
    public static IServiceCollection AddWarehouse(this IServiceCollection services)
    {
        return services;
    }

    public static IBusRegistrationConfigurator ConfigureWarehouse(this
        IBusRegistrationConfigurator configure)
    {
        configure.AddConsumer<ReserveProductCommandConsumer>();
        
        return configure;
    }
}