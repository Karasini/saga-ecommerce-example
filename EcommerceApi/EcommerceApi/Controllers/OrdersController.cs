using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orders.Commands;
using Orders.Queries;
using Orders.Services;
using Payments.Commands;
using Payments.Services;

namespace EcommerceApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{

    private readonly IOrdersService _ordersService;
    private readonly IPaymentsService _paymentsService;

    public OrdersController(IOrdersService ordersService, IPaymentsService paymentsService)
    {
        _ordersService = ordersService;
        _paymentsService = paymentsService;
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
    
    [HttpPost("{id:int}/payments")]
    public async Task<IActionResult> MakePayment(MakePayment payment)
    {
        await _paymentsService.MakePayment(payment);
        return NoContent();
    }
}