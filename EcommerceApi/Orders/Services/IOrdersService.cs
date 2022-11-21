using Orders.Commands;

namespace Orders.Services;

public interface IOrdersService
{
    Task CreateOrder(CreateOrder createOrder);
}