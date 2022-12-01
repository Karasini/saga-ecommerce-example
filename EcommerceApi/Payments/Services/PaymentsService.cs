using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Payments.Commands;

namespace Payments.Services;

internal class PaymentsService : IPaymentsService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentsService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task MakePayment(MakePayment command)
    {
        if (command.Result == "success")
        {
            await _publishEndpoint.Publish(new PaymentSucceeded(command.OrderId, DateTime.Now));
        }
        else
        {
            await _publishEndpoint.Publish(new PaymentFailed(command.OrderId));
        }
    }
}