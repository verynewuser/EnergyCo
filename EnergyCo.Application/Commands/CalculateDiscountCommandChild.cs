namespace EnergyCo.Application.Commands;

public class CalculateDiscountCommandChild
{
    public required string ProductId { get; set; }
    public decimal UnitPrice  { get; set; }
    public decimal Quantity { get; set; }
}