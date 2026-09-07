using Moq;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Application.Services;
using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;

namespace Vitalia.Application.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;

    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _cartRepositoryMock = new Mock<ICartRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();

        _service = new OrderService(
            _orderRepositoryMock.Object,
            _cartRepositoryMock.Object,
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object
        );
    }

    [Fact]
    public async Task BuscarPedido_IdInexistente_DeveRetornarNull()
    {
        // Arrange
        const long orderId = 999;

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _service.GetByIdAsync(orderId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task BuscarPedido_DeOutroUsuario_DeveLancarUnauthorizedAccessException()
    {
        // Arrange
        const long orderId = 1;
        const long ownerId = 10;
        const long currentUserId = 4;

        var order = new Order(ownerId);

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(currentUserId);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetByIdAsync(orderId)
            );

        // Assert
        Assert.Equal(
            "Você não tem permissão para acessar este pedido.",
            exception.Message
        );
    }

    [Fact]
    public async Task BuscarPedido_ProprioUsuario_DeveRetornarPedido()
    {
        // Arrange
        const long orderId = 1;
        const long userId = 4;

        var order = new Order(userId);

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(userId);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _service.GetByIdAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(OrderStatus.PENDING, result.Status);
    }

    [Fact]
    public async Task ListarPedidosUsuario_DeveUsarUserIdDoUsuarioAutenticado()
    {
        // Arrange
        const long userId = 4;

        var orders = new List<Order>
        {
            new(userId),
            new(userId)
        };

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(userId);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByUserIdAsync(userId))
            .ReturnsAsync(orders);

        // Act
        var result =
            (await _service.GetCurrentUserOrdersAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        _orderRepositoryMock.Verify(
            repository =>
                repository.GetByUserIdAsync(userId),
            Times.Once
        );
    }

    [Fact]
    public async Task Checkout_CarrinhoInexistente_DeveLancarKeyNotFoundERollback()
    {
        // Arrange
        const long cartId = 999;

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync((Cart?)null);

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CheckoutAsync(cartId)
            );

        // Assert
        Assert.Equal(
            $"Carrinho com ID {cartId} não encontrado.",
            exception.Message
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.BeginTransactionAsync(),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackTransactionAsync(),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.CommitTransactionAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task Checkout_CarrinhoDeOutroUsuario_DeveLancarUnauthorizedAccessException()
    {
        // Arrange
        const long cartId = 1;
        const long ownerId = 10;
        const long currentUserId = 4;

        var cart = new Cart(ownerId);

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(currentUserId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        // Act
        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.CheckoutAsync(cartId)
            );

        // Assert
        Assert.Equal(
            "Você não tem permissão para finalizar este carrinho.",
            exception.Message
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackTransactionAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task Checkout_CarrinhoNaoAtivo_DeveLancarInvalidOperationException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;

        var cart = new Cart(userId);
        cart.Checkout();

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CheckoutAsync(cartId)
            );

        // Assert
        Assert.Equal(
            "O carrinho não está ativo.",
            exception.Message
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackTransactionAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task Checkout_CarrinhoVazio_DeveLancarInvalidOperationException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;

        var cart = new Cart(userId);

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CheckoutAsync(cartId)
            );

        // Assert
        Assert.Equal(
            "Não é possível finalizar um carrinho vazio.",
            exception.Message
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackTransactionAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task Checkout_ProdutoInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 999;

        var cart = new Cart(userId);

        cart.AddItem(
            new CartItem(
                cartId,
                productId,
                1,
                50m
            )
        );

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(userId);

        _cartRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(cartId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CheckoutAsync(cartId)
            );

        // Assert
        Assert.Equal(
            $"Produto com ID {productId} não encontrado.",
            exception.Message
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackTransactionAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task Checkout_EstoqueInsuficiente_DeveLancarInvalidOperationException()
    {
        // Arrange
        const long cartId = 1;
        const long userId = 4;
        const long productId = 1;

        var cart = new Cart(userId);

        cart.AddItem(
            new CartItem(
                cartId,
                productId,
                10,
                50m
            )
        );

        var product = new Product(
            1,
            "Produto Teste",
            "Produto",
            50m,
            5
        );

        _currentUserMock
            .SetupGet(user => user.UserId)
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
                _service.CheckoutAsync(cartId)
            );

        // Assert
        Assert.Equal(
            $"Estoque insuficiente para o produto '{product.Name}'.",
            exception.Message
        );

        Assert.Equal(5, product.Stock);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackTransactionAsync(),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.CommitTransactionAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task ConfirmarPedido_PedidoInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        const long orderId = 999;

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.ConfirmAsync(orderId)
            );

        // Assert
        Assert.Equal(
            $"Pedido com ID {orderId} não encontrado.",
            exception.Message
        );

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(It.IsAny<Order>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task ConfirmarPedido_AdminPodeGerenciarPedidoDeOutroUsuario()
    {
        // Arrange
        const long orderId = 1;

        var order = new Order(10);

        _currentUserMock
            .Setup(user =>
                user.IsInRole("ADMIN"))
            .Returns(true);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.ConfirmAsync(orderId);

        // Assert
        Assert.Equal(
            OrderStatus.CONFIRMED,
            order.Status
        );

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(order),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task ConfirmarPedido_UsuarioNaoAdminPedidoDeOutroUsuario_DeveLancarUnauthorizedAccessException()
    {
        // Arrange
        const long orderId = 1;
        const long ownerId = 10;
        const long currentUserId = 4;

        var order = new Order(ownerId);

        _currentUserMock
            .Setup(user =>
                user.IsInRole("ADMIN"))
            .Returns(false);

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(currentUserId);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.ConfirmAsync(orderId)
            );

        // Assert
        Assert.Equal(
            "Você não tem permissão para acessar este pedido.",
            exception.Message
        );

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(It.IsAny<Order>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ProcessarPedido_Confirmado_DeveAtualizarParaProcessing()
    {
        // Arrange
        const long orderId = 1;
        const long userId = 4;

        var order = new Order(userId);
        order.Confirm();

        _currentUserMock
            .Setup(user =>
                user.IsInRole("ADMIN"))
            .Returns(true);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.ProcessAsync(orderId);

        // Assert
        Assert.Equal(
            OrderStatus.PROCESSING,
            order.Status
        );

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(order),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task EnviarPedido_Processando_DeveAtualizarParaShipped()
    {
        // Arrange
        const long orderId = 1;

        var order = new Order(4);

        order.Confirm();
        order.Process();

        _currentUserMock
            .Setup(user =>
                user.IsInRole("ADMIN"))
            .Returns(true);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.ShipAsync(orderId);

        // Assert
        Assert.Equal(
            OrderStatus.SHIPPED,
            order.Status
        );

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(order),
            Times.Once
        );
    }

    [Fact]
    public async Task EntregarPedido_Enviado_DeveAtualizarParaDelivered()
    {
        // Arrange
        const long orderId = 1;

        var order = new Order(4);

        order.Confirm();
        order.Process();
        order.Ship();

        _currentUserMock
            .Setup(user =>
                user.IsInRole("ADMIN"))
            .Returns(true);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.DeliverAsync(orderId);

        // Assert
        Assert.Equal(
            OrderStatus.DELIVERED,
            order.Status
        );

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(order),
            Times.Once
        );
    }

    [Fact]
    public async Task CancelarPedido_ProprioPedido_DeveAtualizarParaCancelled()
    {
        // Arrange
        const long orderId = 1;
        const long userId = 4;

        var order = new Order(userId);

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(userId);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.CancelAsync(orderId);

        // Assert
        Assert.Equal(
            OrderStatus.CANCELLED,
            order.Status
        );

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(order),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task CancelarPedido_DeOutroUsuario_DeveLancarUnauthorizedAccessException()
    {
        // Arrange
        const long orderId = 1;
        const long ownerId = 10;
        const long currentUserId = 4;

        var order = new Order(ownerId);

        _currentUserMock
            .SetupGet(user => user.UserId)
            .Returns(currentUserId);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.CancelAsync(orderId)
            );

        // Assert
        Assert.Equal(
            "Você não tem permissão para acessar este pedido.",
            exception.Message
        );

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(It.IsAny<Order>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never
        );
    }
    
    [Fact]
public async Task Checkout_CarrinhoValido_DeveCriarPedidoAtualizarEstoqueEFinalizarCarrinho()
{
    // Arrange
    const long cartId = 1;
    const long userId = 4;
    const long productId = 1;

    var cart = new Cart(userId);

    var product = new Product(
        1,
        "Ração Premium",
        "Produto para teste",
        50m,
        10
    );

    cart.AddItem(
        new CartItem(
            cartId,
            productId,
            2,
            product.Price
        )
    );

    // Pedido que simularemos como retornado pelo banco
    var persistedOrder = new Order(userId);
    persistedOrder.SetTotalAmount(100m);

    _currentUserMock
        .SetupGet(user => user.UserId)
        .Returns(userId);

    _cartRepositoryMock
        .Setup(repository =>
            repository.GetByIdAsync(cartId))
        .ReturnsAsync(cart);

    _productRepositoryMock
        .Setup(repository =>
            repository.GetByIdAsync(productId))
        .ReturnsAsync(product);

    _orderRepositoryMock
        .Setup(repository =>
            repository.AddAsync(It.IsAny<Order>()))
        .Returns(Task.CompletedTask);

    _orderRepositoryMock
        .Setup(repository =>
            repository.GetByIdAsync(It.IsAny<long>()))
        .ReturnsAsync(persistedOrder);

    _unitOfWorkMock
        .Setup(unitOfWork =>
            unitOfWork.SaveChangesAsync())
        .ReturnsAsync(1);

    // Act
    var result =
        await _service.CheckoutAsync(cartId);

    // Assert
    Assert.NotNull(result);

    Assert.Equal(userId, result.UserId);
    Assert.Equal(100m, result.TotalAmount);
    Assert.Equal(OrderStatus.PENDING, result.Status);

    // 10 em estoque - 2 comprados = 8
    Assert.Equal(8, product.Stock);

    Assert.Equal(
        CartStatus.CHECKED_OUT,
        cart.Status
    );

    _orderRepositoryMock.Verify(
        repository =>
            repository.AddAsync(
                It.Is<Order>(order =>
                    order.UserId == userId &&
                    order.TotalAmount == 100m &&
                    order.Items.Count == 1)),
        Times.Once
    );

    _productRepositoryMock.Verify(
        repository =>
            repository.Update(product),
        Times.Once
    );

    _cartRepositoryMock.Verify(
        repository =>
            repository.Update(cart),
        Times.Once
    );

    _unitOfWorkMock.Verify(
        unitOfWork =>
            unitOfWork.BeginTransactionAsync(),
        Times.Once
    );

    _unitOfWorkMock.Verify(
        unitOfWork =>
            unitOfWork.SaveChangesAsync(),
        Times.Once
    );

    _unitOfWorkMock.Verify(
        unitOfWork =>
            unitOfWork.CommitTransactionAsync(),
        Times.Once
    );

    _unitOfWorkMock.Verify(
        unitOfWork =>
            unitOfWork.RollbackTransactionAsync(),
        Times.Never
    );
}
}