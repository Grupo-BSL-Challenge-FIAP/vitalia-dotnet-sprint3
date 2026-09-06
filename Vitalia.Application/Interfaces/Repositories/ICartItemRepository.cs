using Vitalia.Domain.Entities;

namespace Vitalia.Application.Interfaces.Repositories;

public interface ICartItemRepository
{
    Task<CartItem?> GetByIdAsync(long id);

    Task<CartItem?> GetByCartAndProductAsync(
        long cartId,
        long productId);

    Task AddAsync(CartItem cartItem);

    void Update(CartItem cartItem);

    void Delete(CartItem cartItem);
}