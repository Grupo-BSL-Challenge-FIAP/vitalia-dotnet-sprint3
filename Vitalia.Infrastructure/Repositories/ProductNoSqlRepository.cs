using MongoDB.Driver;
using Vitalia.Application.DTOs.Product;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Infrastructure.NoSql.Documents;

namespace Vitalia.Infrastructure.Repositories;

public class ProductNoSqlRepository : IProductNoSqlRepository
{
    private readonly IMongoCollection<ProductDocument> _products;

    public ProductNoSqlRepository(IMongoDatabase database)
    {
        _products = database.GetCollection<ProductDocument>("products");
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _products
            .Find(Builders<ProductDocument>.Filter.Empty)
            .ToListAsync();

        return products.Select(MapToResponse);
    }

    public async Task<ProductResponse?> GetByProductIdAsync(int productId)
    {
        var product = await _products
            .Find(p => p.ProductId == productId)
            .FirstOrDefaultAsync();

        return product is null
            ? null
            : MapToResponse(product);
    }

    public async Task<IEnumerable<ProductResponse>> GetByStatusAsync(string status)
    {
        var products = await _products
            .Find(p => p.Status == status)
            .ToListAsync();

        return products.Select(MapToResponse);
    }

    private static ProductResponse MapToResponse(ProductDocument product)
    {
        return new ProductResponse
        {
            Id = product.ProductId,
            CategoryId = product.Category.CategoryId,
            Name = product.Name,
            Description = product.Description,
            Price = Convert.ToDecimal(product.Price),
            Stock = product.Stock,
            Status = product.Status,
            CreatedAt = ParseDate(product.CreatedAt),
            UpdatedAt = ParseDate(product.UpdatedAt)
        };
    }

    private static DateTime ParseDate(string value)
    {
        return DateTime.TryParse(value, out var date)
            ? date
            : DateTime.MinValue;
    }
}