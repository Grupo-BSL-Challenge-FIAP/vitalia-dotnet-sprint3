using Microsoft.EntityFrameworkCore;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;
using Vitalia.Infrastructure.Data;

namespace Vitalia.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly VitaliaDbContext _context;

    public CartRepository(VitaliaDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetActiveByUserIdAsync(long userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.Status == CartStatus.ACTIVE);
    }

    public async Task<Cart?> GetByIdAsync(long id)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    public void Update(Cart cart)
    {
        _context.Carts.Update(cart);
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Carts
            .AnyAsync(c => c.Id == id);
    }
}