using AutoMapper;
using EnergyCo.Application.Common.Mapping;
using EnergyCo.Domain.Entities;
using EnergyCo.Domain.Enums;

namespace EnergyCo.Application.DTOs;

public class ProductDto: IMapFrom<Product>
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public Category Category { get; set; }
    public List<DiscountPromotionDto> DiscountPromotions{ get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Product, ProductDto>()
               .ForMember(d => d.ProductId, opt => opt.MapFrom(src => src.ProductId.Trim()))
               .ForMember(d => d.DiscountPromotions, opt => opt.MapFrom(src => src.DiscountPromotions.ToList()));
    }
}