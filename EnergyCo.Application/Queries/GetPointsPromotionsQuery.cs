using EnergyCo.Application.DTOs;
using EnergyCo.Application.Common.Interfaces;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace EnergyCo.Application.Queries;

public class GetPointsPromotionsQuery: IRequest<List<PointsPromotionDto>>
{
}
public class GetPointsPromotionsQueryHanlder : IRequestHandler<GetPointsPromotionsQuery, List<PointsPromotionDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetPointsPromotionsQueryHanlder(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<List<PointsPromotionDto>> Handle(GetPointsPromotionsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _applicationDbContext
                                .PointsPromotions                                    
                                .AsNoTracking()
                                .ToListAsync();

        return entities.Select(e => new PointsPromotionDto
        {
            PointsPromotionId = e.PointsPromotionId,
            Category = e.Category,
            PointsPerDollar = e.PointsPerDollar
        }).ToList();

        /* 
         * Due to some weird AutoMapper error, I am manually mapping above.
         * The commented line below is how it should ideally be.
         */

        //return _mapper.Map<List<PointsPromotionDto>>(entities);
    }
}