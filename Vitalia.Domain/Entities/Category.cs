using Vitalia.Domain.Common;

namespace Vitalia.Domain.Entities;

public class Category : Entity
{
    public string Name { get; private set; } = String.Empty;

    public string? Description { get; private set; }
}