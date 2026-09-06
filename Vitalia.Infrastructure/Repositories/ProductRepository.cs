using Microsoft.EntityFrameworkCore;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;
using Vitalia.Infrastructure.Data;

namespace Vitalia.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly VitaliaDbContext _context;

    public ProductRepository(VitaliaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(long categoryId)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Products
            .AnyAsync(p => p.Id == id);
    }
}