using System;
using MassTransit;

namespace Saga;

public class CheckoutState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public int OrderId { get; set; }
    public int DeliveryId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int ProductId { get; set; }
    public int PaymentRetries { get; set; }
    
    public int RequestCount { get; set; }
    public Guid? OrderPaymentTimeoutTokenId { get; set; }

}