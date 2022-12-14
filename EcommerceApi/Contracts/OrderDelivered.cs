namespace Contracts;

public record OrderDelivered
{
    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
}