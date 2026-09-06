namespace Vitalia.Application.DTOs.Category;

public class CategoryResponse
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
