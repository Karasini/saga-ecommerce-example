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
                .Then(x => LogStep(logger, nameof(OrderStatusRequest), x.Saga))
                .TransitionTo(Created));

        DuringAny(
            When(OrderStatusRequest)
                .Then(x => LogStep(logger, nameof(OrderStatusRequest), x.Saga))
                .Then(x => x.Saga.RequestCount += 1)
                .Then(x =>
                {
                    if (x.Saga.RequestCount % 3 == 0)
                    {
                        throw new Exception();
                    }
                })
                .RespondAsync(x => x.Init<OrderStatusResponse>(new OrderStatusResponse
                {
                    Status = x.Saga.CurrentState,
                    OrderId = x.Saga.OrderId
                })));
    }

    private void LogStep(ILogger<CheckoutStateMachine> logger, string stepName, CheckoutState state)
    {
        logger.LogInformation(
            "{StepName} with correlationId: {CorrelationId}, orderId: {OrderId} requestCount: {RequestCount}", stepName, state.CorrelationId,
            state.OrderId, state.RequestCount);
    }
}