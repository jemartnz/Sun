using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;

public sealed class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(p => p.Description)
               .HasMaxLength(1000);

        // Value Object Price
        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(p => p.Amount)
                 .HasColumnName("PriceAmount")
                 .HasColumnType("decimal(18,2)");

            price.Property(p => p.Currency)
                 .HasColumnName("PriceCurrency")
                 .HasMaxLength(3);
        });
    }
}
