namespace Contracts;

public record ReserveProductCommand
{
    public int ProductId { get; init; }
    public int OrderId { get; init; }
}