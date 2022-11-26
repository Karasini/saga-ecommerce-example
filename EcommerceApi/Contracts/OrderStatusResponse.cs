namespace Contracts;

public record OrderStatusResponse
{
    public int OrderId { get; init; }
    public string Status { get; init; }
}