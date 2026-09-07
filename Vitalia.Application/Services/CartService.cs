using Vitalia.Application.DTOs.Cart;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;

namespace Vitalia.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CartService(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<CartResponse> GetOrCreateActiveCartAsync()
    {
        var userId = _currentUser.UserId;

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

        if (cart is null)
        {
            return null;
        }

        ValidateCartOwnership(cart);

        return MapToResponse(cart);
    }

    public async Task<CartResponse> AddItemAsync(
        long cartId,
        CartItemRequest request)
    {
        var cart = await GetOwnedCartAsync(cartId);

        if (cart.Status != CartStatus.ACTIVE)
        {
            throw new InvalidOperationException(
                "Somente um carrinho ativo pode ser alterado."
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

        if (product.Status != ProductStatus.ACTIVE)
        {
            throw new InvalidOperationException(
                "O produto não está disponível para venda."
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

        cart.Touch();

        await _unitOfWork.SaveChangesAsync();

        var updatedCart = await GetOwnedCartAsync(cartId);

        return MapToResponse(updatedCart);
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

        var cart = await GetOwnedCartAsync(cartId);

        if (cart.Status != CartStatus.ACTIVE)
        {
            throw new InvalidOperationException(
                "Somente um carrinho ativo pode ser alterado."
            );
        }

        var product = await _productRepository
            .GetByIdAsync(productId);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Produto com ID {productId} não encontrado."
            );
        }

        if (product.Status != ProductStatus.ACTIVE)
        {
            throw new InvalidOperationException(
                "O produto não está disponível para venda."
            );
        }

        if (quantity > product.Stock)
        {
            throw new InvalidOperationException(
                "A quantidade solicitada é maior que o estoque disponível."
            );
        }

        var item = await _cartItemRepository
            .GetByCartAndProductAsync(
                cartId,
                productId);

        if (item is null)
        {
            throw new KeyNotFoundException(
                "Produto não encontrado no carrinho."
            );
        }

        item.UpdateQuantity(quantity);

        _cartItemRepository.Update(item);

        cart.Touch();

        await _unitOfWork.SaveChangesAsync();

        var updatedCart = await GetOwnedCartAsync(cartId);

        return MapToResponse(updatedCart);
    }

    public async Task RemoveItemAsync(
        long cartId,
        long productId)
    {
        var cart = await GetOwnedCartAsync(cartId);

        if (cart.Status != CartStatus.ACTIVE)
        {
            throw new InvalidOperationException(
                "Somente um carrinho ativo pode ser alterado."
            );
        }

        var item = await _cartItemRepository
            .GetByCartAndProductAsync(
                cartId,
                productId);

        if (item is null)
        {
            throw new KeyNotFoundException(
                "Produto não encontrado no carrinho."
            );
        }

        _cartItemRepository.Delete(item);

        cart.Touch();

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Cart> GetOwnedCartAsync(long cartId)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);

        if (cart is null)
        {
            throw new KeyNotFoundException(
                $"Carrinho com ID {cartId} não encontrado."
            );
        }

        ValidateCartOwnership(cart);

        return cart;
    }

    private void ValidateCartOwnership(Cart cart)
    {
        if (cart.UserId != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException(
                "Você não tem permissão para acessar este carrinho."
            );
        }
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