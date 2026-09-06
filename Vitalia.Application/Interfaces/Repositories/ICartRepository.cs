using Vitalia.Domain.Entities;

namespace Vitalia.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetActiveByUserIdAsync(long userId);

    Task<Cart?> GetByIdAsync(long id);

    Task AddAsync(Cart cart);

    void Update(Cart cart);

    Task<bool> ExistsAsync(long id);
}