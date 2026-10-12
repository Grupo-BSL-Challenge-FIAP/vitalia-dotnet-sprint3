using Vitalia.Application.DTOs.Product;

namespace Vitalia.Application.Interfaces.Repositories;

public interface IProductNoSqlRepository
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();

    Task<ProductResponse?> GetByProductIdAsync(int productId);

    Task<IEnumerable<ProductResponse>> GetByStatusAsync(string status);
}