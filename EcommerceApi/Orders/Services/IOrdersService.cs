using Orders.Commands;
using Orders.DTO;
using Orders.Queries;

namespace Orders.Services;

public interface IOrdersService
{
    Task CreateOrder(CreateOrder createOrder);
    Task<OrderDto> GetOrder(GetOrder query);
}