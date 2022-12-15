using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Delivery.Services;

internal class DeliveryService : IDeliveryService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public DeliveryService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task MakeDelivery(int deliveryId)
    {
        await _publishEndpoint.Publish(new DeliverySucceeded { OrderId = deliveryId });
    }
}