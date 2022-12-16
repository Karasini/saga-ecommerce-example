using Microsoft.Extensions.DependencyInjection;
using Orders.Services;

namespace Orders;

public static class Extensions
{
    public static IServiceCollection AddOrders(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersService>();
        
        return services;
    }
}