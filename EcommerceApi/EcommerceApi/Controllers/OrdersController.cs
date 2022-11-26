using Microsoft.AspNetCore.Mvc;
using Orders.Commands;
using Orders.Queries;
using Orders.Services;

namespace EcommerceApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{

    private readonly IOrdersService _ordersService;

    public OrdersController(IOrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrders(int id)
    {
        var orderDto = await _ordersService.GetOrder(new GetOrder(id));
        return Ok(orderDto);
    }
    
    [HttpPost()]
    public async Task<IActionResult> CreateOrder(CreateOrder createOrder)
    {
        await _ordersService.CreateOrder(createOrder);
        return NoContent();
    }
}