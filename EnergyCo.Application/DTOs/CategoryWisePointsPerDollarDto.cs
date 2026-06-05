using EnergyCo.Domain.Enums;

namespace EnergyCo.Application.DTOs;

public class CategoryWisePointsPerDollarDto
{
    public Category Category { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PointsPerDollar { get; set; }        
}