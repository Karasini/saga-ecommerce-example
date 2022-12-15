using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payments.Commands;

namespace EcommerceApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DeliveryController : ControllerBase
{
    [HttpPost("{id:int}")]
    public async Task<IActionResult> ConfirmDelivery(MakePayment payment)
    {
        return NoContent();
    }
}