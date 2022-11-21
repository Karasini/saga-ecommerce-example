using MassTransit;
using Orders.Commands;
using Orders.Events;

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
            CorrelationId = Guid.NewGuid()
        };

        await _publishEndpoint.Publish(createdEvent);
    }
}