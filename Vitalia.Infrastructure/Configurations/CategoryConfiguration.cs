using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitalia.Domain.Entities;

namespace Vitalia.Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("TB_VITALIA_PRODUCT_CATEGORY");
        
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("CATEGORY_ID");
        
        builder.Property(c => c.Name)
            .HasColumnName("NAME")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(c => c.Description)
            .HasColumnName("DESCRIPTION")
            .HasMaxLength(500);
    }
}