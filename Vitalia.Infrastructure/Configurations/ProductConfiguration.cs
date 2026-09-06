using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitalia.Domain.Entities;

namespace Vitalia.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("TB_VITALIA_PRODUCT");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("PRODUCT_ID");

        builder.Property(p => p.CategoryId)
            .HasColumnName("CATEGORY_ID")
            .IsRequired();

        builder.Property(p => p.Name)
            .HasColumnName("NAME")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("DESCRIPTION")
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .HasColumnName("PRICE")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(p => p.Stock)
            .HasColumnName("STOCK")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("STATUS")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UPDATED_AT")
            .IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId);
    }
}