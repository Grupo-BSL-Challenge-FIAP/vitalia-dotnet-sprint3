using Vitalia.Application.DTOs.Order;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;

namespace Vitalia.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<OrderResponse?> GetByIdAsync(long id)
    {
        var order = await GetOwnedOrderOrNullAsync(id);

        return order is null
            ? null
            : MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponse>> GetCurrentUserOrdersAsync()
    {
        var orders = await _orderRepository
            .GetByUserIdAsync(_currentUser.UserId);

        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse> CheckoutAsync(long cartId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);

            if (cart is null)
            {
                throw new KeyNotFoundException(
                    $"Carrinho com ID {cartId} não encontrado.");
            }

            if (cart.UserId != _currentUser.UserId)
            {
                throw new UnauthorizedAccessException(
                    "Você não tem permissão para finalizar este carrinho.");
            }

            if (cart.Status != CartStatus.ACTIVE)
            {
                throw new InvalidOperationException(
                    "O carrinho não está ativo.");
            }

            if (!cart.Items.Any())
            {
                throw new InvalidOperationException(
                    "Não é possível finalizar um carrinho vazio.");
            }

            var order = new Order(cart.UserId);

            decimal total = 0;

            foreach (var cartItem in cart.Items)
            {
                var product = await _productRepository
                    .GetByIdAsync(cartItem.ProductId);

                if (product is null)
                {
                    throw new KeyNotFoundException(
                        $"Produto com ID {cartItem.ProductId} não encontrado.");
                }

                if (product.Status != ProductStatus.ACTIVE)
                {
                    throw new InvalidOperationException(
                        $"O produto '{product.Name}' não está disponível para venda.");
                }

                if (cartItem.Quantity > product.Stock)
                {
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para o produto '{product.Name}'.");
                }

                var orderItem = new OrderItem(
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

            await _unitOfWork.CommitTransactionAsync();

            var createdOrder = await _orderRepository.GetByIdAsync(order.Id);

            if (createdOrder is null)
            {
                throw new InvalidOperationException(
                    "O pedido foi criado, mas não pôde ser recuperado.");
            }

            return MapToResponse(createdOrder);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task ConfirmAsync(long orderId)
    {
        var order = await GetOwnedOrderAsync(orderId);

        order.Confirm();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ProcessAsync(long orderId)
    {
        var order = await GetOwnedOrderAsync(orderId);

        order.Process();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ShipAsync(long orderId)
    {
        var order = await GetOwnedOrderAsync(orderId);

        order.Ship();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeliverAsync(long orderId)
    {
        var order = await GetOwnedOrderAsync(orderId);

        order.Deliver();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CancelAsync(long orderId)
    {
        var order = await GetOwnedOrderAsync(orderId);

        order.Cancel();

        _orderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Order> GetOwnedOrderAsync(long orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
        {
            throw new KeyNotFoundException(
                $"Pedido com ID {orderId} não encontrado.");
        }

        ValidateOrderOwnership(order);

        return order;
    }

    private async Task<Order?> GetOwnedOrderOrNullAsync(long orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
        {
            return null;
        }

        ValidateOrderOwnership(order);

        return order;
    }

    private void ValidateOrderOwnership(Order order)
    {
        if (order.UserId != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException(
                "Você não tem permissão para acessar este pedido.");
        }
    }

    private static OrderResponse MapToResponse(Order order)
    {
        var items = order.Items
            .Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.Subtotal
            })
            .ToList();

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
}