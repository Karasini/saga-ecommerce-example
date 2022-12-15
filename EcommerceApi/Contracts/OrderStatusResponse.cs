using System;

namespace Contracts;

public record OrderStatusResponse
{
    public int OrderId { get; init; }
    public int? DeliveryId { get; set; }
    public string CurrentState { get; init; }
    public DateTime? PaymentDate { get; set; }
    public int ProductId { get; set; }
    public int PaymentRetries { get; set; }
}