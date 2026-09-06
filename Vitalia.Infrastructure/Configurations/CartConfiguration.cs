using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitalia.Domain.Entities;

namespace Vitalia.Infrastructure.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("TB_VITALIA_CART");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("CART_ID");

        builder.Property(c => c.UserId)
            .HasColumnName("USER_ID")
            .IsRequired();

        builder.Property(c => c.Status)
            .HasColumnName("STATUS")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("UPDATED_AT")
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>();
    }
}