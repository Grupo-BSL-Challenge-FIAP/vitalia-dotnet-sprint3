using Vitalia.Domain.Common;

namespace Vitalia.Domain.Entities;

public class OrderItem : Entity
{
    public long OrderId { get; private set; }

    public long ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Subtotal { get; private set; }

    public Order Order { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    public OrderItem(
        long productId,
        int quantity,
        decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "A quantidade deve ser maior que zero.");

        if (unitPrice <= 0)
            throw new ArgumentException(
                "O preço unitário deve ser maior que zero.");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Subtotal = quantity * unitPrice;
    }
}