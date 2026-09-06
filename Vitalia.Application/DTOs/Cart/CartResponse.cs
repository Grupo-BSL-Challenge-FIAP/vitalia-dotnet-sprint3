namespace Vitalia.Application.DTOs.Cart;

public class CartResponse
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public decimal Total { get; set; }

    public IEnumerable<CartItemResponse> Items { get; set; }
        = new List<CartItemResponse>();
}