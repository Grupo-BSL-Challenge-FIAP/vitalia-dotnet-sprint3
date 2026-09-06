using Microsoft.EntityFrameworkCore;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Domain.Entities;
using Vitalia.Infrastructure.Data;

namespace Vitalia.Infrastructure.Repositories;

public class CartItemRepository : ICartItemRepository
{
    private readonly VitaliaDbContext _context;

    public CartItemRepository(VitaliaDbContext context)
    {
        _context = context;
    }

    public async Task<CartItem?> GetByIdAsync(long id)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == id);
    }

    public async Task<CartItem?> GetByCartAndProductAsync(
        long cartId,
        long productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci =>
                ci.CartId == cartId &&
                ci.ProductId == productId);
    }

    public async Task AddAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
    }

    public void Update(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
    }

    public void Delete(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
    }
}