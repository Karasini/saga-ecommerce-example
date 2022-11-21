using MassTransit;

namespace Orders.Events;

public record OrderCreated() : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; }
    public int OrderId { get; init; }
    public int ProductId { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
}