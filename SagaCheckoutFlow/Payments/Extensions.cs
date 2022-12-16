using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Payments.Consumers;
using Payments.Services;

namespace Payments;

public static class Extensions
{
    public static IServiceCollection AddPayments(this IServiceCollection services)
    {
        services.AddScoped<IPaymentsService, PaymentsService>();
        
        return services;
    }
    
    public static IBusRegistrationConfigurator ConfigurePayments(this
        IBusRegistrationConfigurator configure)
    {
        configure.AddConsumer<RefundMoneyCommandConsumer>();
        
        return configure;
    }
}