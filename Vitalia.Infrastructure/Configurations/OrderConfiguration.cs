using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitalia.Domain.Entities;

namespace Vitalia.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("TB_VITALIA_ORDERS");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("ORDER_ID");

        builder.Property(o => o.UserId)
            .HasColumnName("USER_ID")
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .HasColumnName("ORDER_DATE")
            .IsRequired();

        builder.Property(o => o.Status)
            .HasColumnName("STATUS")
            .HasMaxLength(30)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.TotalAmount)
            .HasColumnName("TOTAL_AMOUNT")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);
    }
}