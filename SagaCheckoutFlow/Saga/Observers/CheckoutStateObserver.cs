using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Saga.Observers;

internal class CheckoutStateObserver : IStateObserver<CheckoutState>
{
    private readonly ILogger<CheckoutStateObserver> _logger;

    public CheckoutStateObserver(ILogger<CheckoutStateObserver> logger)
    {
        _logger = logger;
    }

    public Task StateChanged(BehaviorContext<CheckoutState> context, State currentState, State previousState)
    {
        _logger.LogInformation("StateChanged '{PreviousState}' => '{CurrentState}'", previousState?.Name,
            currentState?.Name);

        return Task.CompletedTask;
    }
}