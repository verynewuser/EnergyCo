using EnergyCo.Domain.Enums;

namespace EnergyCo.Application.DTOs;

public class BasketGroupByCategoryDto
{
    public Category Category { get; set; }
    public decimal Amount { get; set; }
    public decimal PointsPerDollar { get; set; }
    public decimal TotalPoints { get; set; }
}