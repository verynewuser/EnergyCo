using EnergyCo.Domain.Common;

namespace EnergyCo.Domain.Entities;

public class DiscountPromotion: Promotion
{
    public required string DiscountPromotionId { get; set; }
    public decimal DiscountPercent { get; set; }
    public ICollection<Product> Products { get; set; }
}