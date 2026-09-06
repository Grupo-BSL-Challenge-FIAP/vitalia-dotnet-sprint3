using Vitalia.Application.DTOs.Cart;

namespace Vitalia.Application.Interfaces.Services;

public interface ICartService
{
    Task<CartResponse> GetOrCreateActiveCartAsync(long userId);

    Task<CartResponse?> GetByIdAsync(long id);

    Task<CartResponse> AddItemAsync(
        long cartId,
        CartItemRequest request);

    Task<CartResponse> UpdateItemAsync(
        long cartId,
        long productId,
        int quantity);

    Task RemoveItemAsync(
        long cartId,
        long productId);

    Task CheckoutAsync(long cartId);
}