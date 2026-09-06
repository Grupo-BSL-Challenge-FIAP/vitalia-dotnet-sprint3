using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Domain.Entities;
using Vitalia.Infrastructure.Data;

namespace Vitalia.Infrastructure.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly VitaliaDbContext _context;

    public OrderItemRepository(VitaliaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
    }
}