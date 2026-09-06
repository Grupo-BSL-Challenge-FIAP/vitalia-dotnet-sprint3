using Vitalia.Application.DTOs.Cart;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Domain.Entities;

namespace Vitalia.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CartResponse> GetOrCreateActiveCartAsync(long userId)
    {
        var cart = await _cartRepository
            .GetActiveByUserIdAsync(userId);

        if (cart is null)
        {
            cart = new Cart(userId);

            await _cartRepository.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        return MapToResponse(cart);
    }

    public async Task<CartResponse?> GetByIdAsync(long id)
    {
        var cart = await _cartRepository.GetByIdAsync(id);

        return cart is null
            ? null
            : MapToResponse(cart);
    }

    public async Task<CartResponse> AddItemAsync(
        long cartId,
        CartItemRequest request)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);

        if (cart is null)
        {
            throw new KeyNotFoundException(
                $"Carrinho com ID {cartId} não encontrado."
            );
        }

        var product = await _productRepository
            .GetByIdAsync(request.ProductId);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Produto com ID {request.ProductId} não encontrado."
            );
        }

        if (request.Quantity <= 0)
        {
            throw new ArgumentException(
                "A quantidade deve ser maior que zero."
            );
        }

        if (request.Quantity > product.Stock)
        {
            throw new InvalidOperationException(
                "A quantidade solicitada é maior que o estoque disponível."
            );
        }

        var existingItem = await _cartItemRepository
            .GetByCartAndProductAsync(
                cartId,
                request.ProductId);

        if (existingItem is not null)
        {
            var newQuantity =
                existingItem.Quantity + request.Quantity;

            if (newQuantity > product.Stock)
            {
                throw new InvalidOperationException(
                    "A quantidade total do produto excede o estoque disponível."
                );
            }

            existingItem.UpdateQuantity(newQuantity);

            _cartItemRepository.Update(existingItem);
        }
        else
        {
            var cartItem = new CartItem(
                cartId,
                request.ProductId,
                request.Quantity,
                product.Price);

            await _cartItemRepository.AddAsync(cartItem);
        }

        await _unitOfWork.SaveChangesAsync();

        var updatedCart = await _cartRepository.GetByIdAsync(cartId);

        return MapToResponse(updatedCart!);
    }

    public async Task<CartResponse> UpdateItemAsync(
        long cartId,
        long productId,
        int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "A quantidade deve ser maior que zero."
            );
        }

        var cart = await _cartRepository.GetByIdAsync(cartId);

        if (cart is null)
        {
            throw new KeyNotFoundException(
                $"Carrinho com ID {cartId} não encontrado."
            );
        }

        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Produto com ID {productId} não encontrado."
            );
        }

        if (quantity > product.Stock)
        {
            throw new InvalidOperationException(
                "A quantidade solicitada é maior que o estoque disponível."
            );
        }

        var item = await _cartItemRepository
            .GetByCartAndProductAsync(cartId, productId);

        if (item is null)
        {
            throw new KeyNotFoundException(
                "Produto não encontrado no carrinho."
            );
        }

        item.UpdateQuantity(quantity);

        _cartItemRepository.Update(item);

        await _unitOfWork.SaveChangesAsync();

        var updatedCart = await _cartRepository.GetByIdAsync(cartId);

        return MapToResponse(updatedCart!);
    }

    public async Task RemoveItemAsync(
        long cartId,
        long productId)
    {
        var item = await _cartItemRepository
            .GetByCartAndProductAsync(cartId, productId);

        if (item is null)
        {
            throw new KeyNotFoundException(
                "Produto não encontrado no carrinho."
            );
        }

        _cartItemRepository.Delete(item);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CheckoutAsync(long cartId)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);

        if (cart is null)
        {
            throw new KeyNotFoundException(
                $"Carrinho com ID {cartId} não encontrado."
            );
        }

        if (!cart.Items.Any())
        {
            throw new InvalidOperationException(
                "Não é possível finalizar um carrinho vazio."
            );
        }

        cart.Checkout();

        _cartRepository.Update(cart);

        await _unitOfWork.SaveChangesAsync();
    }

    private static CartResponse MapToResponse(Cart cart)
    {
        var items = cart.Items
            .Select(item => new CartItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.Quantity * item.UnitPrice
            })
            .ToList();

        return new CartResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Status = cart.Status.ToString(),
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Total = items.Sum(item => item.Subtotal),
            Items = items
        };
    }
}