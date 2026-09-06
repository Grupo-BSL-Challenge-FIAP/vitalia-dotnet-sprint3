using Microsoft.EntityFrameworkCore;

namespace Vitalia.Infrastructure.Data;

public class VitaliaDbContext : DbContext
{
    public VitaliaDbContext(DbContextOptions<VitaliaDbContext> options) : base(options)
    {
        
    }
}