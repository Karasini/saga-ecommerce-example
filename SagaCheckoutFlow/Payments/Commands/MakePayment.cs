namespace Payments.Commands;

public record MakePayment
{
    public int OrderId { get; set; }
    public string Result { get; set; }
}