using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitalia.Domain.Entities;

namespace Vitalia.Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("TB_VITALIA_ORDER_ITEM");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .HasColumnName("ORDER_ITEM_ID");

        builder.Property(oi => oi.OrderId)
            .HasColumnName("ORDER_ID")
            .IsRequired();

        builder.Property(oi => oi.ProductId)
            .HasColumnName("PRODUCT_ID")
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .HasColumnName("QUANTITY")
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .HasColumnName("UNIT_PRICE")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(oi => oi.Subtotal)
            .HasColumnName("SUBTOTAL")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);

        builder.HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);
    }
}