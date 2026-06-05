namespace EnergyCo.Domain.Common;

public class Promotion
{ 
    public required string PromotionName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}