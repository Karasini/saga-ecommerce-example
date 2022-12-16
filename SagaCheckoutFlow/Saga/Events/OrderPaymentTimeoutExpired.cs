namespace Saga.Events;

public record OrderPaymentTimeoutExpired
{
    public int OrderId { get; set; }
}