using Vitalia.Application.DTOs.Product;
using Vitalia.Domain.Enums;

namespace Vitalia.Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();

    Task<ProductResponse?> GetByIdAsync(long id);

    Task<ProductResponse> AddAsync(ProductRequest request);

    Task UpdateAsync(long id, ProductRequest request);

    Task DeleteAsync(long id);
    
    Task<IEnumerable<ProductResponse>> GetByCategoryIdAsync(long categoryId);

    Task<IEnumerable<ProductResponse>> GetByStatusAsync(ProductStatus status);
}