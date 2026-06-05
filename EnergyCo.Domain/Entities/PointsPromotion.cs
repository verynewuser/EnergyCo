using EnergyCo.Domain.Common;
using EnergyCo.Domain.Enums;

namespace EnergyCo.Domain.Entities;

public class PointsPromotion: Promotion
{
    public string PointsPromotionId { get; set; }
    public Category Category { get; set; }
    public string CategoryName => ((Category)this.Category).ToString();
    public decimal PointsPerDollar { get; set; }        
}
