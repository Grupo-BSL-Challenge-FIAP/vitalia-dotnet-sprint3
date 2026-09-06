using Vitalia.Domain.Common;
using Vitalia.Domain.Enums;

namespace Vitalia.Domain.Entities;

public class Cart : Entity
{
    public long UserId { get; private set; }

    public CartStatus Status { get; private set; } = CartStatus.Active;

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
    
    
    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();
}