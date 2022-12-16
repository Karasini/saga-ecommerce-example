using Automatonymous;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Saga.Observers;

namespace Saga;

public static class Extensions
{
    public static IServiceCollection AddSaga(this IServiceCollection services)
    {
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