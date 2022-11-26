namespace Contracts;

public record OrderCreated()
{
    public int OrderId { get; init; }
    public int ProductId { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
}