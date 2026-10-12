using Vitalia.Domain.Entities;

namespace Vitalia.Domain.Tests;

public class OrderItemTests
{
    [Fact]
    public void CriarOrderItem_ComDadosValidos_DeveCalcularSubtotal()
    {
        // Arrange
        const long productId = 1;
        const int quantity = 3;
        const decimal unitPrice = 50m;

        // Act
        var item = new OrderItem(
            productId,
            quantity,
            unitPrice);

        // Assert
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(unitPrice, item.UnitPrice);
        Assert.Equal(150m, item.Subtotal);
    }

    [Fact]
    public void CriarOrderItem_ComQuantidadeZero_DeveLancarExcecao()
    {
        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            new OrderItem(
                1,
                0,
                50m)
        );
    }

    [Fact]
    public void CriarOrderItem_ComQuantidadeNegativa_DeveLancarExcecao()
    {
        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            new OrderItem(
                1,
                -1,
                50m)
        );
    }

    [Fact]
    public void CriarOrderItem_ComPrecoZero_DeveLancarExcecao()
    {
        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            new OrderItem(
                1,
                1,
                0m)
        );
    }

    [Fact]
    public void CriarOrderItem_ComPrecoNegativo_DeveLancarExcecao()
    {
        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            new OrderItem(
                1,
                1,
                -10m)
        );
    }
}