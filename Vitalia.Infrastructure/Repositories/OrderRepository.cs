using Microsoft.EntityFrameworkCore;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Domain.Entities;
using Vitalia.Infrastructure.Data;

namespace Vitalia.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly VitaliaDbContext _context;

    public OrderRepository(VitaliaDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(long id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(long userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == id);
    }
}