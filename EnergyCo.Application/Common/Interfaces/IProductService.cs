using EnergyCo.Domain.Entities;

namespace EnergyCo.Application.Common.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetProductDiscountPromotions(List<string> productIds);
    Task<List<PointsPromotion>> GetPointsPromotions();
}