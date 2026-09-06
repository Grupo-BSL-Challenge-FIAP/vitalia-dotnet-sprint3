using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitalia.Domain.Entities;

namespace Vitalia.Infrastructure.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("TB_VITALIA_CART_ITEM");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .HasColumnName("CART_ITEM_ID");

        builder.Property(ci => ci.CartId)
            .HasColumnName("CART_ID")
            .IsRequired();

        builder.Property(ci => ci.ProductId)
            .HasColumnName("PRODUCT_ID")
            .IsRequired();

        builder.Property(ci => ci.Quantity)
            .HasColumnName("QUANTITY")
            .IsRequired();

        builder.Property(ci => ci.UnitPrice)
            .HasColumnName("UNIT_PRICE")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId);

        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId);

        builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();
    }
}