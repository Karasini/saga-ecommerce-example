namespace Contracts;

public record ProductReserved
{
    public int ProductId { get; init; }
    public int OrderId { get; init; }
}