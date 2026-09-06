using Vitalia.Domain.Common;
using Vitalia.Domain.Enums;

namespace Vitalia.Domain.Entities;

public class Order : Entity
{
    public long UserId { get; private set; }

    public DateTime OrderDate { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public ICollection<OrderItem> Items { get; private set; }
        = new List<OrderItem>();

    public Order(long userId)
    {
        UserId = userId;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.PENDING;
        TotalAmount = 0;
    }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
    }

    public void SetTotalAmount(decimal totalAmount)
    {
        TotalAmount = totalAmount;
    }

    public void Confirm()
    {
        Status = OrderStatus.CONFIRMED;
    }

    public void Process()
    {
        Status = OrderStatus.PROCESSING;
    }

    public void Ship()
    {
        Status = OrderStatus.SHIPPED;
    }

    public void Deliver()
    {
        Status = OrderStatus.DELIVERED;
    }

    public void Cancel()
    {
        Status = OrderStatus.CANCELLED;
    }
}