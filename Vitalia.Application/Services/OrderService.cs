using Vitalia.Application.DTOs.Order;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;

namespace Vitalia.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse?> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<OrderResponse>> GetByUserIdAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderResponse> CheckoutAsync(long cartId)
    {
        throw new NotImplementedException();
    }
}