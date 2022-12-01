namespace Orders.Commands;

public record CreateOrder
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string Color { get; set; }
    public string Size { get; set; }
}