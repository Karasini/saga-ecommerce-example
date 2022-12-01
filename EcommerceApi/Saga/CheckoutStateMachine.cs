using System;
using Contracts;
using MassTransit;
using MassTransit.Events;
using Microsoft.Extensions.Logging;
using Saga.Events;

namespace Saga;

public class CheckoutStateMachine : MassTransitStateMachine<CheckoutState>
{
    public State Created { get; private set; }
    public State Paid { get; private set; }
    public State PaymentFailed { get; private set; }
    public State Reserved { get; private set; }
    public State ReservationFailed { get; private set; }
    public State DeliveryBooked { get; private set; }
    public State Cancelled { get; private set; }
    public State Closed { get; private set; }
    public State Completed { get; private set; }

    public Event<Fault<OrderCreated>> FaultOrderCreated { get; private set; }
    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<PaymentSucceeded> PaymentSucceeded { get; private set; }
    public Event<PaymentFailed> PaymentFailedEvent { get; private set; }
    public Event<OrderStatusRequest> OrderStatusRequest { get; private set; }
    public Schedule<CheckoutState, OrderPaymentTimeoutExpired> OrderPaymentTimeout { get; private set; }

    public CheckoutStateMachine(ILogger<CheckoutStateMachine> logger)
    {
        SetupEvents();

        Schedule(() => OrderPaymentTimeout, instance => instance.OrderPaymentTimeoutTokenId, s =>
        {
            s.Delay = TimeSpan.FromMinutes(1);

            s.Received = r => r.CorrelateBy<int>(state => state.OrderId, m => m.Message.OrderId);
        });
        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderCreated)
                .Then(x =>
                {
                    x.Saga.OrderId = x.Message.OrderId;
                    x.Saga.ProductId = x.Message.ProductId;
                    x.Saga.Color = x.Message.Color;
                    x.Saga.Size = x.Message.Size;
                })
                // .Schedule(OrderPaymentTimeout,
                //     context => context.Init<OrderPaymentTimeoutExpired>(new OrderPaymentTimeoutExpired { OrderId = context.Saga.OrderId }))
                .TransitionTo(Created));

        During(Created,
            When(PaymentSucceeded)
                .Then(x => x.Saga.PaymentDate = x.Message.PaymentDate)
                .TransitionTo(Paid));

        During(Created, PaymentFailed, When(PaymentFailedEvent)
            .Then(x => x.Saga.PaymentRetries += 1)
            .IfElse(x => x.Saga.PaymentRetries >= 3,
                x => x.TransitionTo(Cancelled),
                x => x.TransitionTo(PaymentFailed)));
        
        DuringAny(
            When(OrderPaymentTimeout?.Received)
                .Unschedule(OrderPaymentTimeout),
            When(FaultOrderCreated).Then(x => LogStep(logger, nameof(FaultOrderCreated), x.Saga)),
            When(OrderStatusRequest)
                .Then(x => x.Saga.RequestCount += 1)
                .Then(x =>
                {
                    // Demo purpose - generate fail scenario to trigger retry
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

    private void SetupEvents()
    {
        Event(() => OrderCreated,
            e => e.CorrelateBy<int>(state => state.OrderId, m => m.Message.OrderId)
                .SelectId(x => x.CorrelationId ?? NewId.NextGuid()));

        Event(() => FaultOrderCreated, e => e.CorrelateBy<int>(state => state.OrderId, m => m.Message.Message.OrderId));
        Event(() => PaymentSucceeded, e => e.CorrelateBy<int>(state => state.OrderId, m => m.Message.OrderId));
        Event(() => PaymentFailedEvent, e => e.CorrelateBy<int>(state => state.OrderId, m => m.Message.OrderId));

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
    }

    private static void LogStep(ILogger<CheckoutStateMachine> logger, string stepName, CheckoutState state)
    {
        logger.LogInformation(
            "{StepName} with correlationId: {CorrelationId}, orderId: {OrderId} requestCount: {RequestCount}", stepName,
            state.CorrelationId,
            state.OrderId, state.RequestCount);
    }
}