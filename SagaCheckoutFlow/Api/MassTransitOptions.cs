namespace Api;

internal class MassTransitOptions
{
    internal enum TransportsTypes
    {
        InMemory,
        RabbitMq
    }
    public TransportsTypes Transport { get; set; }
}