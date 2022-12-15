using System;
using System.Linq;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Saga.Events;

namespace Saga;

public class CheckoutStateMachine : MassTransitStateMachine<CheckoutState>
{
    public State Created { get; private set; }
    public State Paid { get; private set; }
    public State PaymentFailed { get; private set; }
    public State ProductReserved { get; private set; }
    public State ReservationFailed { get; private set; }
    public State DeliveryBooked { get; private set; }
    public State BookDeliveryFailed { get; private set; }
    public State Cancelled { get; private set; }
    public State Closed { get; private set; }
    public State Completed { get; private set; }

    public Event<Fault<OrderCreated>> FaultOrderCreated { get; private set; }
    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<PaymentSucceeded> PaymentSucceeded { get; private set; }
    public Event<PaymentFailed> PaymentFailedEvent { get; private set; }
    public Event<OrderStatusRequest> OrderStatusRequest { get; private set; }
    public Schedule<CheckoutState, OrderPaymentTimeoutExpired> OrderPaymentTimeout { get; private set; }
    public Event<ProductReserved> ProductReservedEvent { get; private set; }
    public Request<CheckoutState, BookDeliveryRequest, BookDeliveryResponse> BookDelivery { get; private set; }
    public Event<DeliverySucceeded> DeliverySucceeded { get; private set; }

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
                })
                // .Schedule(OrderPaymentTimeout,
                //     context => context.Init<OrderPaymentTimeoutExpired>(new OrderPaymentTimeoutExpired { OrderId = context.Saga.OrderId }))
                .TransitionTo(Created));

        During(Created, PaymentFailed,
            When(PaymentSucceeded)
                .Then(x => x.Saga.PaymentDate = x.Message.PaymentDate)
                .PublishAsync(x => x.Init<ReserveProductCommand>(new ReserveProductCommand
                {
                    ProductId = x.Saga.ProductId,
                    OrderId = x.Saga.OrderId
                }))
                .TransitionTo(Paid),
            When(PaymentFailedEvent)
                .Then(x => x.Saga.PaymentRetries += 1)
                .IfElse(x => x.Saga.PaymentRetries >= 3,
                    x => x.TransitionTo(Cancelled),
                    x => x.TransitionTo(PaymentFailed)));

        During(Paid,
            When(ProductReservedEvent)
                .Request(BookDelivery, x => x.Init<BookDeliveryRequest>(new BookDeliveryRequest()))
                .TransitionTo(BookDelivery.Pending));

        During(BookDelivery.Pending,
            When(BookDelivery.Completed)
                .Then(x => x.Saga.DeliveryId = x.Message.DeliveryId)
                .TransitionTo(DeliveryBooked),
            When(BookDelivery.Faulted)
                .TransitionTo(BookDeliveryFailed),
            When(BookDelivery.TimeoutExpired)
                .TransitionTo(BookDeliveryFailed)); //TODO: Cancel order and refund

        During(DeliveryBooked,
            When(DeliverySucceeded)
                .TransitionTo(Completed));
        
        
        DuringAny(
            When(OrderPaymentTimeout?.Received)
                .Unschedule(OrderPaymentTimeout).TransitionTo(Cancelled),
            When(FaultOrderCreated).Then(x =>
                logger.LogInformation("Something went wrong with Handling OrderCreated: {Exception}",
                    x.Message.Exceptions.FirstOrDefault().Message)),
            When(OrderStatusRequest)
                .Then(x => x.Saga.RequestCount += 1)
                .Then(x =>
                {
                    // Demo purpose - generate fail scenario to trigger retry
                    // if (x.Saga.RequestCount % 3 == 0)
                    // {
                    //     throw new Exception();
                    // }
                })
                .RespondAsync(x => x.Init<OrderStatusResponse>(new OrderStatusResponse
                {
                    CurrentState = x.Saga.CurrentState,
                    OrderId = x.Saga.OrderId,
                    DeliveryId = x.Saga.DeliveryId,
                    PaymentDate = x.Saga.PaymentDate,
                    PaymentRetries = x.Saga.PaymentRetries,
                    ProductId = x.Saga.ProductId
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
        Event(() => ProductReservedEvent, e => e.CorrelateBy<int>(state => state.OrderId, m => m.Message.OrderId));
        Event(() => DeliverySucceeded, e => e.CorrelateBy<int>(state => state.DeliveryId, m => m.Message.DeliveryId));

        Request(() => BookDelivery);

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
}