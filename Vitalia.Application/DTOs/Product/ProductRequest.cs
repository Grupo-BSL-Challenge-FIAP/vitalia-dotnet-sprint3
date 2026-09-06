namespace Vitalia.Application.DTOs.Product;

public class ProductRequest
{
    public long CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }
}