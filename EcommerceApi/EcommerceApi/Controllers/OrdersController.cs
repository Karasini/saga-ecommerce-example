using Microsoft.AspNetCore.Mvc;
using Orders.Commands;
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

    [HttpGet()]
    public IActionResult GetOrders()
    {
        return Ok("Orders get");
    }
    
    [HttpPost()]
    public async Task<IActionResult> CreateOrder(CreateOrder createOrder)
    {
        await _ordersService.CreateOrder(createOrder);
        return NoContent();
    }
}