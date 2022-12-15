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
        // using var scope = _logger.BeginScope("CheckoutState for {OrderId} changes", context.Saga.OrderId);
        _logger.LogInformation($"StateChanged '{previousState}' => '{currentState}'");

        return Task.CompletedTask;
    }
}