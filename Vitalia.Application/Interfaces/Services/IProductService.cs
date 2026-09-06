using Vitalia.Application.DTOs.Product;
using Vitalia.Domain.Entities;

namespace Vitalia.Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(long id);

    Task AddAsync(ProductRequest request);

    Task UpdateAsync(Product product);

    Task DeleteAsync(long id);
}