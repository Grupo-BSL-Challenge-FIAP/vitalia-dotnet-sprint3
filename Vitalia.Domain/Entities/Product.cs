using Vitalia.Domain.Common;
using Vitalia.Domain.Enums;

namespace Vitalia.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; }

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public ProductStatus Status { get; private set; }

    public long CategoryId { get; private set; }

    public Category Category { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Product(
        long categoryId,
        string name,
        string? description,
        decimal price,
        int stock)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        Status = ProductStatus.ACTIVE;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}