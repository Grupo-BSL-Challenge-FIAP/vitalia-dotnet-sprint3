using Microsoft.EntityFrameworkCore;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Domain.Entities;
using Vitalia.Infrastructure.Data;

namespace Vitalia.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly VitaliaDbContext _context;

    public CategoryRepository(VitaliaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(long id)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public void Delete(Category category)
    {
        _context.Categories.Remove(category);
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id);
    }
}
