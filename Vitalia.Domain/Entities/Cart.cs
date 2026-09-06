using Vitalia.Domain.Common;
using Vitalia.Domain.Enums;

namespace Vitalia.Domain.Entities;

public class Cart : Entity
{
    public long UserId { get; private set; }

    public CartStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    public Cart(long userId)
    {
        UserId = userId;
        Status = CartStatus.ACTIVE;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddItem(CartItem item)
    {
        Items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(CartItem item)
    {
        Items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Checkout()
    {
        if (Status != CartStatus.ACTIVE)
            throw new InvalidOperationException(
                "Somente um carrinho ativo pode ser finalizado.");

        Status = CartStatus.CHECKED_OUT;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Abandon()
    {
        Status = CartStatus.ABANDONED;
        UpdatedAt = DateTime.UtcNow;
    }
}