using System.ComponentModel.DataAnnotations;

namespace Vitalia.Application.DTOs.Cart;

public class CartItemRequest
{
    [Range(
        1,
        long.MaxValue,
        ErrorMessage = "O produto deve ser informado."
    )]
    public long ProductId { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "A quantidade deve ser maior que zero."
    )]
    public int Quantity { get; set; }
}