using EnergyCo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnergyCo.Infrastructure.Persistence.Configurations;

public class DiscountPromotionConfiguration: IEntityTypeConfiguration<DiscountPromotion>
{
    public void Configure(EntityTypeBuilder<DiscountPromotion> builder)
    {
        builder.Property(t => t.DiscountPromotionId)
               .HasMaxLength(10)
               .HasColumnType("char");

        builder.Property(t => t.DiscountPercent)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnType("Decimal(18,2)");
    }
}
