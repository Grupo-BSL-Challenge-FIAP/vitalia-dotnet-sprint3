using Vitalia.Domain.Common;

namespace Vitalia.Domain.Entities;

public class CartItem : Entity
{
    public long CartId { get; private set; }

    public long ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public Cart Cart { get; private set; } = null!;

    public Product Product { get; private set; } = null!;
}