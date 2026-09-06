using Vitalia.Domain.Common;
using Vitalia.Domain.Enums;

namespace Vitalia.Domain.Entities;

public class Order : Entity
{
    public long UserId { get; private set; }

    public DateTime OrderDate { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.PENDING;

    public decimal TotalAmount { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
}