using Vitalia.Domain.Entities;

namespace Vitalia.Domain.Tests;

public class CartItemTests
{
    [Fact]
    public void CriarCartItem_ComDadosValidos_DeveCriarItem()
    {
        // Arrange
        const long cartId = 1;
        const long productId = 2;
        const int quantity = 3;
        const decimal unitPrice = 50m;

        // Act
        var item = new CartItem(
            cartId,
            productId,
            quantity,
            unitPrice);

        // Assert
        Assert.Equal(cartId, item.CartId);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(unitPrice, item.UnitPrice);
    }

    [Fact]
    public void AtualizarQuantidade_ComValorValido_DeveAlterarQuantidade()
    {
        // Arrange
        var item = new CartItem(
            1,
            2,
            1,
            50m);

        // Act
        item.UpdateQuantity(5);

        // Assert
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void AtualizarQuantidade_ComZero_DeveLancarExcecao()
    {
        // Arrange
        var item = new CartItem(
            1,
            2,
            1,
            50m);

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            item.UpdateQuantity(0)
        );
    }

    [Fact]
    public void AtualizarQuantidade_ComValorNegativo_DeveLancarExcecao()
    {
        // Arrange
        var item = new CartItem(
            1,
            2,
            1,
            50m);

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            item.UpdateQuantity(-1)
        );
    }
}