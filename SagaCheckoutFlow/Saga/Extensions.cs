using System.Runtime.CompilerServices;
using Automatonymous;
using Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Saga.Observers;

[assembly: InternalsVisibleTo("Tests")]

namespace Saga;

public static class Extensions
{
    public static IServiceCollection AddSaga(this IServiceCollection services)
    {
        var sagaOptions = services.GetOptions<CheckoutSagaOptions>("CheckoutSaga");
        
        services.AddSingleton(sagaOptions);
        services.AddStateObserver<CheckoutState, CheckoutStateObserver>();
        services.AddEventObserver<CheckoutState, CheckoutEventObserver>();
        return services;
    }

    public static IBusRegistrationConfigurator ConfigureSaga(this
        IBusRegistrationConfigurator configure)
    {
        configure.AddSagaStateMachine<CheckoutStateMachine, CheckoutState>(typeof(CheckoutStateMachineDefinition))
            .InMemoryRepository();

        return configure;
    }
}