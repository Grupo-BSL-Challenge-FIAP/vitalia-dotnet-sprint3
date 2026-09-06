namespace Vitalia.Application.DTOs.Product;

public class ProductPagedResponse
{
    public IEnumerable<ProductResponse> Items { get; set; } = [];

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}