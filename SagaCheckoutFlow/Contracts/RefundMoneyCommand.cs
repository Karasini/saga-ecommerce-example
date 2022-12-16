namespace Contracts;

public record RefundMoneyCommand
{
    public int OrderId { get; init; }
}