using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;

namespace Vitalia.Domain.Tests;

public class CartTests
{
    [Fact]
    public void CriarCart_ComUserIdValido_DeveCriarCarrinhoAtivo()
    {
        // Arrange
        const long userId = 4;

        // Act
        var cart = new Cart(userId);

        // Assert
        Assert.Equal(userId, cart.UserId);
        Assert.Equal(CartStatus.ACTIVE, cart.Status);
        Assert.Empty(cart.Items);
        Assert.NotEqual(default, cart.CreatedAt);
        Assert.NotEqual(default, cart.UpdatedAt);
    }

    [Fact]
    public void AdicionarItem_DeveAdicionarItemAoCarrinho()
    {
        // Arrange
        var cart = new Cart(4);

        var item = new CartItem(
            1,
            2,
            1,
            50m);

        // Act
        cart.AddItem(item);

        // Assert
        Assert.Single(cart.Items);
        Assert.Contains(item, cart.Items);
    }

    [Fact]
    public void RemoverItem_DeveRemoverItemDoCarrinho()
    {
        // Arrange
        var cart = new Cart(4);

        var item = new CartItem(
            1,
            2,
            1,
            50m);

        cart.AddItem(item);

        // Act
        cart.RemoveItem(item);

        // Assert
        Assert.Empty(cart.Items);
    }

    [Fact]
    public void FinalizarCart_Ativo_DeveAlterarStatusParaFinalizado()
    {
        // Arrange
        var cart = new Cart(4);

        // Act
        cart.Checkout();

        // Assert
        Assert.Equal(
            CartStatus.CHECKED_OUT,
            cart.Status);
    }

    [Fact]
    public void FinalizarCart_JaFinalizado_DeveLancarExcecao()
    {
        // Arrange
        var cart = new Cart(4);
        cart.Checkout();

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            cart.Checkout()
        );
    }

    [Fact]
    public void FinalizarCart_Abandonado_DeveLancarExcecao()
    {
        // Arrange
        var cart = new Cart(4);
        cart.Abandon();

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            cart.Checkout()
        );
    }

    [Fact]
    public void AbandonarCart_DeveAlterarStatusParaAbandonado()
    {
        // Arrange
        var cart = new Cart(4);

        // Act
        cart.Abandon();

        // Assert
        Assert.Equal(
            CartStatus.ABANDONED,
            cart.Status);
    }

    [Fact]
    public void Touch_DeveManterCarrinhoValido()
    {
        // Arrange
        var cart = new Cart(4);

        // Act
        cart.Touch();

        // Assert
        Assert.NotEqual(
            default,
            cart.UpdatedAt);
    }
}