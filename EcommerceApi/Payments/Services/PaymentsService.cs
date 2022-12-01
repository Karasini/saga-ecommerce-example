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
        var successEvent = new PaymentSucceeded(command.OrderId, DateTime.Now);

        await _publishEndpoint.Publish(successEvent);
    }
}