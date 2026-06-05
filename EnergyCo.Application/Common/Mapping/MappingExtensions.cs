using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace EnergyCo.Application.Common.Mapping;

public static class MappingExtensions
{
    public static List<TDestination> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration)
    {
        return queryable.ProjectTo<TDestination>(configuration).ToList();
    }
}