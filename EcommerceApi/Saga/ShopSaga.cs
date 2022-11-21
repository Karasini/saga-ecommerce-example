using MassTransit;
using Orders.Events;

namespace Saga;

public class ShopSaga : ISaga, InitiatedBy<OrderCreated>
{
    public Guid CorrelationId { get; set; }
    
    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        Console.WriteLine("TODO");
        
        return Task.CompletedTask;
    }
}