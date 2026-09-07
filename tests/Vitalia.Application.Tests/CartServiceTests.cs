using Moq;
using Vitalia.Application.DTOs.Cart;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Application.Services;
using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;

namespace Vitalia.Application.Tests;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<ICartItemRepository> _cartItemRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;

    private readonly CartService _service;

    public CartServiceTests()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();
        _cartItemRepositoryMock = new Mock<ICartItemRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();

        _service = new CartService(
            _cartRepositoryMock.Object,
            _cartItemRepositoryMock.Object,
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object
        );
    }
    

    [Fact]
    public async Task ObterCarrinho_UsuarioSemCarrinhoAtivo_DeveCriarNovoCarrinho()
    {
        // Arrange
        const long userId = 4;

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetActiveByUserIdAsync(userId))
            .ReturnsAsync((Cart?)null);

        _cartRepositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Cart>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result =
            await _service.GetOrCreateActiveCartAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("ACTIVE", result.Status);
        Assert.Equal(0m, result.Total);
        Assert.Empty(result.Items);

        _cartRepositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.Is<Cart>(cart =>
                        cart.UserId == userId &&
                        cart.Status == CartStatus.ACTIVE)),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task ObterCarrinho_UsuarioComCarrinhoAtivo_DeveRetornarCarrinhoExistente()
    {
        // Arrange
        const long userId = 4;

        var cart = new Cart(userId);

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetActiveByUserIdAsync(userId))
            .ReturnsAsync(cart);

        // Act
        var result =
            await _service.GetOrCreateActiveCartAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("ACTIVE", result.Status);

        _cartRepositoryMock.Verify(
            repository =>
                repository.AddAsync(It.IsAny<Cart>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task BuscarCarrinho_IdInexistente_DeveRetornarNull()
    {
        // Arrange
        const long cartId = 999;

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync((Cart?)null);

        // Act
        var result =
            await _service.GetByIdAsync(cartId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task BuscarCarrinho_DeOutroUsuario_DeveLancarUnauthorizedAccessException()
    {
        // Arrange
        const long cartId = 1;
        const long ownerId = 10;
        const long currentUserId = 4;

        var cart = new Cart(ownerId);

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(currentUserId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        // Act
        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetByIdAsync(cartId)
            );

        // Assert
        Assert.Equal(
            "Você não tem permissão para acessar este carrinho.",
            exception.Message
        );
    }

    [Fact]
    public async Task AdicionarItem_CarrinhoInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        const long cartId = 999;
        const long userId = 4;

        var request = new CartItemRequest
        {
            ProductId = 1,
            Quantity = 1
        };

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync((Cart?)null);

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.AddItemAsync(cartId, request)
            );

        // Assert
        Assert.Equal(
            $"Carrinho com ID {cartId} não encontrado.",
            exception.Message
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task AdicionarItem_CarrinhoFinalizado_DeveLancarInvalidOperationException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;

        var cart = new Cart(userId);
        cart.Checkout();

        var request = new CartItemRequest
        {
            ProductId = 1,
            Quantity = 1
        };

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddItemAsync(cartId, request)
            );

        // Assert
        Assert.Equal(
            "Somente um carrinho ativo pode ser alterado.",
            exception.Message
        );
    }

    [Fact]
    public async Task AdicionarItem_ProdutoInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;

        var cart = new Cart(userId);

        var request = new CartItemRequest
        {
            ProductId = 999,
            Quantity = 1
        };

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(request.ProductId))
            .ReturnsAsync((Product?)null);

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.AddItemAsync(cartId, request)
            );

        // Assert
        Assert.Equal(
            $"Produto com ID {request.ProductId} não encontrado.",
            exception.Message
        );

        _cartItemRepositoryMock.Verify(
            repository =>
                repository.AddAsync(It.IsAny<CartItem>()),
            Times.Never
        );
    }

    [Fact]
    public async Task AdicionarItem_QuantidadeInvalida_DeveLancarArgumentException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;

        var cart = new Cart(userId);

        var product = new Product(
            1,
            "Produto",
            "Produto teste",
            50m,
            10
        );

        var request = new CartItemRequest
        {
            ProductId = 1,
            Quantity = 0
        };

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(request.ProductId))
            .ReturnsAsync(product);

        // Act
        var exception =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddItemAsync(cartId, request)
            );

        // Assert
        Assert.Equal(
            "A quantidade deve ser maior que zero.",
            exception.Message
        );
    }

    [Fact]
    public async Task AdicionarItem_EstoqueInsuficiente_DeveLancarInvalidOperationException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;

        var cart = new Cart(userId);

        var product = new Product(
            1,
            "Produto",
            "Produto teste",
            50m,
            5
        );

        var request = new CartItemRequest
        {
            ProductId = 1,
            Quantity = 10
        };

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(request.ProductId))
            .ReturnsAsync(product);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddItemAsync(cartId, request)
            );

        // Assert
        Assert.Equal(
            "A quantidade solicitada é maior que o estoque disponível.",
            exception.Message
        );
    }

    [Fact]
    public async Task AdicionarItem_ProdutoNovo_DeveAdicionarItemESalvar()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        var product = new Product(
            1,
            "Ração Premium",
            "Produto teste",
            149.90m,
            20
        );

        var request = new CartItemRequest
        {
            ProductId = productId,
            Quantity = 2
        };

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _cartItemRepositoryMock
            .Setup(repository =>
                repository.GetByCartAndProductAsync(
                    cartId,
                    productId))
            .ReturnsAsync((CartItem?)null);

        _cartItemRepositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<CartItem>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result =
            await _service.AddItemAsync(cartId, request);

        // Assert
        Assert.NotNull(result);

        _cartItemRepositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.Is<CartItem>(item =>
                        item.ProductId == productId &&
                        item.Quantity == request.Quantity &&
                        item.UnitPrice == product.Price)),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task AdicionarItem_ProdutoJaNoCarrinho_DeveSomarQuantidade()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        var product = new Product(
            1,
            "Produto",
            "Produto teste",
            50m,
            10
        );

        var existingItem = new CartItem(
            cartId,
            productId,
            2,
            product.Price
        );

        var request = new CartItemRequest
        {
            ProductId = productId,
            Quantity = 3
        };

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _cartItemRepositoryMock
            .Setup(repository =>
                repository.GetByCartAndProductAsync(
                    cartId,
                    productId))
            .ReturnsAsync(existingItem);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.AddItemAsync(cartId, request);

        // Assert
        Assert.Equal(5, existingItem.Quantity);

        _cartItemRepositoryMock.Verify(
            repository =>
                repository.Update(existingItem),
            Times.Once
        );

        _cartItemRepositoryMock.Verify(
            repository =>
                repository.AddAsync(It.IsAny<CartItem>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task AdicionarItem_QuantidadeTotalExcedeEstoque_DeveLancarInvalidOperationException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        var product = new Product(
            1,
            "Produto",
            "Produto teste",
            50m,
            5
        );

        var existingItem = new CartItem(
            cartId,
            productId,
            4,
            product.Price
        );

        var request = new CartItemRequest
        {
            ProductId = productId,
            Quantity = 2
        };

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _cartItemRepositoryMock
            .Setup(repository =>
                repository.GetByCartAndProductAsync(
                    cartId,
                    productId))
            .ReturnsAsync(existingItem);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddItemAsync(cartId, request)
            );

        // Assert
        Assert.Equal(
            "A quantidade total do produto excede o estoque disponível.",
            exception.Message
        );

        _cartItemRepositoryMock.Verify(
            repository =>
                repository.Update(It.IsAny<CartItem>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task AtualizarItem_QuantidadeInvalida_DeveLancarArgumentException()
    {
        // Arrange
        const long cartId = 1;
        const long productId = 1;

        // Act
        var exception =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UpdateItemAsync(
                    cartId,
                    productId,
                    0)
            );

        // Assert
        Assert.Equal(
            "A quantidade deve ser maior que zero.",
            exception.Message
        );

        _cartRepositoryMock.Verify(
            repository =>
                repository.GetByIdAsync(It.IsAny<long>()),
            Times.Never
        );
    }

    [Fact]
    public async Task AtualizarItem_ItemInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        var product = new Product(
            1,
            "Produto",
            "Produto teste",
            50m,
            20
        );

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _cartItemRepositoryMock
            .Setup(repository =>
                repository.GetByCartAndProductAsync(
                    cartId,
                    productId))
            .ReturnsAsync((CartItem?)null);

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateItemAsync(
                    cartId,
                    productId,
                    5)
            );

        // Assert
        Assert.Equal(
            "Produto não encontrado no carrinho.",
            exception.Message
        );
    }

    [Fact]
    public async Task AtualizarItem_EstoqueInsuficiente_DeveLancarInvalidOperationException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        var product = new Product(
            1,
            "Produto",
            "Produto teste",
            50m,
            5
        );

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateItemAsync(
                    cartId,
                    productId,
                    10)
            );

        // Assert
        Assert.Equal(
            "A quantidade solicitada é maior que o estoque disponível.",
            exception.Message
        );
    }

    [Fact]
    public async Task AtualizarItem_Valido_DeveAtualizarQuantidadeESalvar()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        var product = new Product(
            1,
            "Produto",
            "Produto teste",
            50m,
            20
        );

        var item = new CartItem(
            cartId,
            productId,
            2,
            product.Price
        );

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _cartItemRepositoryMock
            .Setup(repository =>
                repository.GetByCartAndProductAsync(
                    cartId,
                    productId))
            .ReturnsAsync(item);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.UpdateItemAsync(
            cartId,
            productId,
            5);

        // Assert
        Assert.Equal(5, item.Quantity);

        _cartItemRepositoryMock.Verify(
            repository =>
                repository.Update(item),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task RemoverItem_ItemInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _cartItemRepositoryMock
            .Setup(repository =>
                repository.GetByCartAndProductAsync(
                    cartId,
                    productId))
            .ReturnsAsync((CartItem?)null);

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.RemoveItemAsync(
                    cartId,
                    productId)
            );

        // Assert
        Assert.Equal(
            "Produto não encontrado no carrinho.",
            exception.Message
        );

        _cartItemRepositoryMock.Verify(
            repository =>
                repository.Delete(It.IsAny<CartItem>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task RemoverItem_ItemExistente_DeveExcluirItemESalvar()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        var item = new CartItem(
            cartId,
            productId,
            2,
            50m
        );

        _currentUserMock
            .SetupGet(currentUser => currentUser.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _cartItemRepositoryMock
            .Setup(repository =>
                repository.GetByCartAndProductAsync(
                    cartId,
                    productId))
            .ReturnsAsync(item);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.RemoveItemAsync(
            cartId,
            productId);

        // Assert
        _cartItemRepositoryMock.Verify(
            repository =>
                repository.Delete(item),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }
}