using EnergyCo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnergyCo.Infrastructure.Persistence.Configurations;

public class ProductConfiguration: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(t => t.ProductId)
            .HasMaxLength(10)
            .HasColumnType("char");

        builder.Property(t => t.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.UnitPrice)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnType("Decimal(18,2)");
    }
}