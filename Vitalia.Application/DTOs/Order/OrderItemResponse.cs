namespace Vitalia.Application.DTOs.Order;

public class OrderItemResponse
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }
}