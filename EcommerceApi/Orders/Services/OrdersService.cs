using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Orders.Commands;
using Orders.DTO;
using Orders.Exceptions;
using Orders.Queries;

namespace Orders.Services;

internal class OrdersService : IOrdersService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRequestClient<OrderStatusRequest> _orderStatusClient;

    public OrdersService(IPublishEndpoint publishEndpoint, IRequestClient<OrderStatusRequest> orderStatusClient)
    {
        _publishEndpoint = publishEndpoint;
        _orderStatusClient = orderStatusClient;
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

    public async Task<OrderDto> GetOrder(GetOrder query)
    {
        var (status, notFound) = await _orderStatusClient.GetResponse<OrderStatusResponse, OrderNotFoundResponse>(new OrderStatusRequest(query.OrderId));

        if (status.IsCompletedSuccessfully)
        {
            var response = await status;
            return new OrderDto(response.Message.OrderId, response.Message.Status);
        }
        else
        {
            var response = await notFound;
            throw new OrderNotFoundException(response.Message.OrderId);
        }
    }
}