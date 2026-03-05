using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;

public sealed class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductId)
               .IsRequired();

        builder.Property(i => i.Quantity)
               .IsRequired();

        builder.OwnsOne(i => i.UnitPrice, price =>
        {
            price.Property(p => p.Amount)
                 .HasColumnName("UnitPriceAmount")
                 .HasColumnType("decimal(18,2)");

            price.Property(p => p.Currency)
                 .HasColumnName("UnitPriceCurrency")
                 .HasMaxLength(3);
        });

        builder.ToTable("OrderItems");
    }
}
