using Microsoft.EntityFrameworkCore;
using Vitalia.Application.Interfaces;
using Vitalia.Domain.Entities;
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

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
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