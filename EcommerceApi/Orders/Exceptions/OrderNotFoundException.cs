namespace Orders.Exceptions;

internal class OrderNotFoundException : Exception
{
    public OrderNotFoundException(int orderId)
    {
        OrderId = orderId;
    }

    public int OrderId { get; private set; }
}