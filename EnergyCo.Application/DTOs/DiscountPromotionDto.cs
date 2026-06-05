using AutoMapper;
using EnergyCo.Application.Common.Mapping;
using EnergyCo.Domain.Entities;

namespace EnergyCo.Application.DTOs;

public class DiscountPromotionDto: IMapFrom<DiscountPromotion>
{
    public required string DiscountPromotionId { get; set; }
    public decimal DiscountPercent { get; set; }
    public required string PromotionName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DiscountPromotion, DiscountPromotionDto>();
    }
}