using System.Threading.Tasks;

namespace Delivery.Services;

public interface IDeliveryService
{
    Task MakeDelivery(int deliveryId);
}