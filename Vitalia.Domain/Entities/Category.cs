using Vitalia.Domain.Common;

namespace Vitalia.Domain.Entities;

public class Category : Entity
{
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Category(
        string name,
        string? description)
    {
        Name = name;
        Description = description;
    }

    public void Update(
        string name,
        string? description)
    {
        Name = name;
        Description = description;
    }
}