using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payments.Commands;
using Payments.Services;

namespace EcommerceApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentsService _service;

    public PaymentsController(IPaymentsService service)
    {
        _service = service;
    }

}