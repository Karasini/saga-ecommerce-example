using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Warehouse.Consumers;

internal class ReserveProductCommandConsumer : IConsumer<ReserveProductCommand>
{
    public async Task Consume(ConsumeContext<ReserveProductCommand> context)
    {
        await Task.Delay(5000);
        await context.Publish(new ProductReserved{ProductId = context.Message.ProductId, OrderId = context.Message.OrderId});
    }
}