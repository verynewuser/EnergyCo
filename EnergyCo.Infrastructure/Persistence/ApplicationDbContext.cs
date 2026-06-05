using EnergyCo.Application.Common.Interfaces;
using EnergyCo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EnergyCo.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<PointsPromotion> PointsPromotions { get; set; }
    public DbSet<DiscountPromotion> DiscountPromotions { get; set; }

    public ApplicationDbContext(DbContextOptions options)
     : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Seed();
        base.OnModelCreating(builder);
    }
}