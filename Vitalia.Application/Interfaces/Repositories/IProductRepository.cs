using Vitalia.Domain.Entities;

namespace Vitalia.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(long id);

    Task AddAsync(Product product);

    void Update(Product product);

    void Delete(Product product);

    Task<bool> ExistsAsync(long id);
}