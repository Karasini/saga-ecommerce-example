using Contracts;
using MassTransit;
using Orders.Commands;

namespace Orders.Services;

public class OrdersService : IOrdersService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrdersService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task CreateOrder(CreateOrder createOrder)
    {
        var createdEvent = new OrderCreated
        {
            OrderId = 1,
            ProductId = createOrder.ProductId,
            Color = createOrder.Color,
            Size = createOrder.Color,
        };

        await _publishEndpoint.Publish(createdEvent);
    }
}