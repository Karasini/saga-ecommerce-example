using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Payments.Consumers;

internal class RefundMoneyCommandConsumer : IConsumer<RefundMoneyCommand>
{
    public async Task Consume(ConsumeContext<RefundMoneyCommand> context)
    {
        await Task.Delay(5000);
        
        await context.Publish(new MoneyRefunded
        {
            OrderId = context.Message.OrderId
        });
    }
}