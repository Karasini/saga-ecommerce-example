using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{

    [HttpGet()]
    public IActionResult GetOrders()
    {
        return Ok("Orders get");
    }
    
    [HttpPost()]
    public IActionResult CreateOrder()
    {
        return Ok("Orders get");
    }
}