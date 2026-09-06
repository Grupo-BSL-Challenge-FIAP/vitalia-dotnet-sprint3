using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;

namespace Vitalia.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(long id);

    Task<IEnumerable<Product>> GetByCategoryIdAsync(long categoryId);

    Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status);

    Task<(IEnumerable<Product> Items, int TotalItems)> GetPagedAsync(
        int page,
        int pageSize);

    Task AddAsync(Product product);

    void Update(Product product);

    void Delete(Product product);

    Task<bool> ExistsAsync(long id);
}