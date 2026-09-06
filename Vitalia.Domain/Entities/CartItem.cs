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

    public CartItem(
        long cartId,
        long productId,
        int quantity,
        decimal unitPrice)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "A quantidade deve ser maior que zero."
            );

        Quantity = quantity;
    }
}