using AutoMapper;
using EnergyCo.Application.Common.Mapping;
using EnergyCo.Domain.Entities;
using EnergyCo.Domain.Enums;

namespace EnergyCo.Application.DTOs;

public class PointsPromotionDto: IMapFrom<PointsPromotion>
{
    public string PointsPromotionId { get; set; }
    public Category Category { get; set; }
    public string CategoryName => ((Category)this.Category).ToString();
    public decimal PointsPerDollar { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<PointsPromotion, PointsPromotionDto>();
    }
}