using AutoMapper;
using EnergyCo.Application.Common.Interfaces;
using EnergyCo.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnergyCo.Application.Queries;

public class GetProductsQuery : IRequest<List<ProductDto>>
{
}

public class GetCategoriesQueryHanlder : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHanlder(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _applicationDbContext
                                .Products
                                .Include(p => p.DiscountPromotions)
                                .AsNoTracking()
                                .ToListAsync(cancellationToken);

        return entities.Select(e => new ProductDto
        {
            Category = e.Category,
            ProductId = e.ProductId,
            ProductName = e.ProductName,
            UnitPrice = e.UnitPrice,
            DiscountPromotions = e.DiscountPromotions.Select(dp => new DiscountPromotionDto
            {
                DiscountPromotionId = dp.DiscountPromotionId,
                DiscountPercent = dp.DiscountPercent,
                EndDate = dp.EndDate,
                StartDate = dp.StartDate,
                PromotionName = dp.PromotionName
            }).ToList()
        }).ToList();

        /* 
         * Due to some weird AutoMapper error, I am manually mapping above.
         * The commented line below is how it should ideally be.
         */

        //return _mapper.Map<List<ProductDto>>(entities);
    }
}