using Vitalia.Domain.Entities;

namespace Vitalia.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(long id);

    Task<IEnumerable<Order>> GetByUserIdAsync(long userId);

    Task AddAsync(Order order);

    void Update(Order order);

    Task<bool> ExistsAsync(long id);
}