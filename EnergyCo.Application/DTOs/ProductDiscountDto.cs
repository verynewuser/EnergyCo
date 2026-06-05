namespace EnergyCo.Application.DTOs;

public class ProductDiscountDto
{
    public required string ProductId { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal Discount { get; set; }
}