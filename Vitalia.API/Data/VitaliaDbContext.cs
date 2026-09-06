using Microsoft.EntityFrameworkCore;

namespace Vitalia.API.Data;

public class VitaliaDbContext : DbContext
{
    public VitaliaDbContext(DbContextOptions<VitaliaDbContext> options) : base(options)
    {
        
    }
}