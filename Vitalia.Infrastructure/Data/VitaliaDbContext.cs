using Microsoft.EntityFrameworkCore;
using Vitalia.Domain.Entities;

namespace Vitalia.Infrastructure.Data;

public class VitaliaDbContext : DbContext
{
    public VitaliaDbContext(DbContextOptions<VitaliaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(VitaliaDbContext).Assembly
        );
    }
}