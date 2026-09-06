using Vitalia.Application.DTOs.Order;

namespace Vitalia.Application.Interfaces.Services;

public interface IOrderService
{
    Task<OrderResponse?> GetByIdAsync(long id);
    Task<IEnumerable<OrderResponse>> GetByUserIdAsync(long userId);
    Task<OrderResponse> CheckoutAsync(long cartId);

    Task ConfirmAsync(long orderId);
    Task ProcessAsync(long orderId);
    Task ShipAsync(long orderId);
    Task DeliverAsync(long orderId);
    Task CancelAsync(long orderId);
}