namespace Orders.Commands;

public record CreateOrder
{
    public int OrderId { get; init; }
}