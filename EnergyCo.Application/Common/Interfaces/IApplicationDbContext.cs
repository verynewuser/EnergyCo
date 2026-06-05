using EnergyCo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnergyCo.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }
    DbSet<PointsPromotion> PointsPromotions { get; set; }
    DbSet<DiscountPromotion> DiscountPromotions { get; set; }
}
