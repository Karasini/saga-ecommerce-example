using Microsoft.Extensions.DependencyInjection;
using Payments.Services;

namespace Payments;

public static class Extensions
{
    public static IServiceCollection AddPayments(this IServiceCollection services)
    {
        services.AddScoped<IPaymentsService, PaymentsService>();
        
        return services;
    }
}