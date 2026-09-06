using Vitalia.Application.DTOs.Order;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Domain.Entities;

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
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return null;

        return MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponse>> GetByUserIdAsync(long userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);

        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse> CheckoutAsync(long cartId)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);

        if (cart is null)
            throw new KeyNotFoundException(
                $"Carrinho com ID {cartId} não encontrado.");

        if (cart.Status != Domain.Enums.CartStatus.ACTIVE)
            throw new InvalidOperationException(
                "O carrinho não está ativo.");

        if (!cart.Items.Any())
            throw new InvalidOperationException(
                "Não é possível finalizar um carrinho vazio.");

        var order = new Domain.Entities.Order(cart.UserId);

        decimal total = 0;

        foreach (var cartItem in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(
                cartItem.ProductId);

            if (product is null)
                throw new KeyNotFoundException(
                    $"Produto com ID {cartItem.ProductId} não encontrado.");

            if (cartItem.Quantity > product.Stock)
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto '{product.Name}'.");

            var orderItem = new Domain.Entities.OrderItem(
                product.Id,
                cartItem.Quantity,
                cartItem.UnitPrice);

            order.AddItem(orderItem);

            total += orderItem.Subtotal;

            product.DecreaseStock(cartItem.Quantity);
            _productRepository.Update(product);
        }

        order.SetTotalAmount(total);

        cart.Checkout();
        _cartRepository.Update(cart);

        await _orderRepository.AddAsync(order);

        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(order);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        var items = order.Items.Select(item => new OrderItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Subtotal = item.Subtotal
        }).ToList();

        return new OrderResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Items = items
        };
    }
    
    public async Task ConfirmAsync(long orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            throw new KeyNotFoundException(
                $"Pedido com ID {orderId} não encontrado.");

        order.Confirm();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ProcessAsync(long orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            throw new KeyNotFoundException(
                $"Pedido com ID {orderId} não encontrado.");

        order.Process();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ShipAsync(long orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            throw new KeyNotFoundException(
                $"Pedido com ID {orderId} não encontrado.");

        order.Ship();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeliverAsync(long orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            throw new KeyNotFoundException(
                $"Pedido com ID {orderId} não encontrado.");

        order.Deliver();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CancelAsync(long orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            throw new KeyNotFoundException(
                $"Pedido com ID {orderId} não encontrado.");

        order.Cancel();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }
}