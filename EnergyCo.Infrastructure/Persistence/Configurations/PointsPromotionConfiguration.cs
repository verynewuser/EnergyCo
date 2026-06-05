using EnergyCo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnergyCo.Infrastructure.Persistence.Configurations;

public class PointsPromotionConfiguration: IEntityTypeConfiguration<PointsPromotion>
{
    public void Configure(EntityTypeBuilder<PointsPromotion> builder)
    {
        builder.Property(t => t.PointsPromotionId)
            .HasMaxLength(10)
            .HasColumnType("char");

        builder.Property(t => t.PointsPerDollar)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnType("Decimal(18,2)");
    }
}