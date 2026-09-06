using Vitalia.Domain.Enums;

namespace Vitalia.Application.DTOs.Order;

public class OrderResponse
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public List<OrderItemResponse> Items { get; set; } = [];
}