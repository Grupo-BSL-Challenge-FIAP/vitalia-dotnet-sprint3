using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Infrastructure.Data;

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

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_context.Database.CurrentTransaction is not null)
        {
            await _context.Database.CurrentTransaction.CommitAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_context.Database.CurrentTransaction is not null)
        {
            await _context.Database.CurrentTransaction.RollbackAsync();
        }
    }
}