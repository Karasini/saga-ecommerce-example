using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Saga;

public class CheckoutStateMachine : MassTransitStateMachine<CheckoutState>
{
    public State Created { get; private set; }
    public State Completed { get; private set; }

    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<OrderStatusRequest> OrderStatusRequest { get; private set; }

    public CheckoutStateMachine(ILogger<CheckoutStateMachine> logger)
    {
        Event(() => OrderCreated,
            e => e.CorrelateBy<int>(state => state.OrderId,
                    m => m.Message.OrderId)
                .SelectId(x => x.CorrelationId ?? NewId.NextGuid()));

        Event(() => OrderStatusRequest, x =>
        {
            x.CorrelateBy<int>(state => state.OrderId,
                m => m.Message.OrderId);

            x.OnMissingInstance(m => m.ExecuteAsync(async context =>
            {
                if (context.RequestId.HasValue)
                {
                    await context.RespondAsync(
                        new OrderNotFoundResponse(context.Message.OrderId));
                }
            }));
        });

        InstanceState(x => x.CurrentState);
        
        Initially(
            When(OrderCreated)
                .Then(x => x.Saga.OrderId = x.Message.OrderId)
                .Then(x => logger.LogInformation("Created checkout saga with correlationId: {correlationId}, orderId: {}", x.Saga.CorrelationId, x.Saga.OrderId))
                .TransitionTo(Created));
        
        DuringAny(
            When(OrderStatusRequest)
                .RespondAsync(x => x.Init<OrderStatusResponse>(new OrderStatusResponse
                {
                    Status = x.Saga.CurrentState,
                    OrderId = x.Saga.OrderId
                })));
    }
}