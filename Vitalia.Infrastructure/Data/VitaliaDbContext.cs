using Microsoft.EntityFrameworkCore;
using Vitalia.Domain.Entities;

namespace Vitalia.Infrastructure.Data;

public class VitaliaDbContext : DbContext
{
    public VitaliaDbContext(DbContextOptions<VitaliaDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
}