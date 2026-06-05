using EnergyCo.Domain.Enums;

namespace EnergyCo.Domain.Entities;

public class Product
{
    public required string ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public Category Category { get; set; }
    public ICollection<DiscountPromotion> DiscountPromotions { get; set; }
}