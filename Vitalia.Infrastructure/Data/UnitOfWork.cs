using Vitalia.Application.Interfaces.Repositories;

namespace Vitalia.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly VitaliaDbContext _context;

    public UnitOfWork(VitaliaDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}