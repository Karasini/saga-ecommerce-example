using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Saga;

public class CheckoutStateMachine : MassTransitStateMachine<CheckoutState>
{
    public State Created { get; private set; }
    public State Completed { get; private set; }

    public Event<OrderCreated> OrderCreated { get; private set; }

    public CheckoutStateMachine(ILogger<CheckoutStateMachine> logger)
    {
        Event(() => OrderCreated,
            e => e.CorrelateBy<int>(state => state.OrderId,
                    m => m.Message.OrderId)
                .SelectId(x => x.CorrelationId ?? NewId.NextGuid()));

        InstanceState(x => x.CurrentState);
        
        Initially(
            When(OrderCreated)
                .Then(x => x.Saga.OrderId = x.Message.OrderId)
                .Then(x => logger.LogInformation("Created checkout saga with correlationId: {correlationId}, orderId: {}", x.Saga.CorrelationId, x.Saga.OrderId))
                .TransitionTo(Created));
    }
}