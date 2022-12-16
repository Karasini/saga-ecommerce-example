using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Delivery.Consumers;

internal class BookDeliveryRequestConsumer : IConsumer<BookDeliveryRequest>
{
    public async Task Consume(ConsumeContext<BookDeliveryRequest> context)
    {
        await Task.Delay(5000);
        await context.RespondAsync(new BookDeliveryResponse
            { DeliveryId = RandomNumberGenerator.GetInt32(int.MaxValue) });
    }
}