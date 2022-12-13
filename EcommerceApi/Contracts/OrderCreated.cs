namespace Contracts;

public record OrderCreated()
{
    public int OrderId { get; init; }
    public int ProductId { get; init; }
}