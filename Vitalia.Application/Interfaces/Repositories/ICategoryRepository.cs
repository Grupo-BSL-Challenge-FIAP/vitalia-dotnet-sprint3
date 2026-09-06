using Vitalia.Domain.Entities;

namespace Vitalia.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(long id);

    Task AddAsync(Category category);

    void Update(Category category);

    void Delete(Category category);

    Task<bool> ExistsAsync(long id);
}
