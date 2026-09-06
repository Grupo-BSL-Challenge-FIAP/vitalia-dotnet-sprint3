using Vitalia.Domain.Entities;

namespace Vitalia.Application.Interfaces.Repositories;

public interface IOrderItemRepository
{
    Task AddAsync(OrderItem orderItem);
}