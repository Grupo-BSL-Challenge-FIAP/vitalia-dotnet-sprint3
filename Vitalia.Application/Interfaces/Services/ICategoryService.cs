using Vitalia.Application.DTOs.Category;

namespace Vitalia.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync();

    Task<CategoryResponse?> GetByIdAsync(long id);

    Task<CategoryResponse> AddAsync(CategoryRequest request);

    Task UpdateAsync(long id, CategoryRequest request);

    Task DeleteAsync(long id);
}
