using System.Threading.Tasks;
using Delivery.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;

    public DeliveryController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    [HttpPost("{id:int}/makeDelivery")]
    public async Task<IActionResult> MakeDelivery(int id)
    {
        await _deliveryService.MakeDelivery(id);
        
        return NoContent();
    }
}