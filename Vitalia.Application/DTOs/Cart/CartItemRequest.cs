namespace Vitalia.Application.DTOs.Cart;

public class CartItemRequest
{
    public long ProductId { get; set; }

    public int Quantity { get; set; }
}