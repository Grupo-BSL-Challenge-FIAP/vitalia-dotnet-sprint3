using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;

namespace Vitalia.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void CriarProduct_ComDadosValidos_DeveCriarProdutoAtivo()
    {
        // Arrange
        const long categoryId = 1;
        const string name = "Ração Premium";
        const string description = "Ração para cães";
        const decimal price = 149.90m;
        const int stock = 20;

        // Act
        var product = new Product(
            categoryId,
            name,
            description,
            price,
            stock);

        // Assert
        Assert.Equal(categoryId, product.CategoryId);
        Assert.Equal(name, product.Name);
        Assert.Equal(description, product.Description);
        Assert.Equal(price, product.Price);
        Assert.Equal(stock, product.Stock);
        Assert.Equal(ProductStatus.ACTIVE, product.Status);
        Assert.NotEqual(default, product.CreatedAt);
        Assert.NotEqual(default, product.UpdatedAt);
    }

    [Fact]
    public void AtualizarProduct_DeveAlterarDados()
    {
        // Arrange
        var product = new Product(
            1,
            "Ração",
            "Descrição inicial",
            100m,
            10);

        // Act
        product.Update(
            2,
            "Ração Atualizada",
            "Descrição atualizada",
            120m,
            30);

        // Assert
        Assert.Equal(2, product.CategoryId);
        Assert.Equal("Ração Atualizada", product.Name);
        Assert.Equal(
            "Descrição atualizada",
            product.Description);
        Assert.Equal(120m, product.Price);
        Assert.Equal(30, product.Stock);
    }

    [Fact]
    public void DiminuirEstoque_DeveReduzirQuantidade()
    {
        // Arrange
        var product = new Product(
            1,
            "Ração",
            "Produto teste",
            100m,
            10);

        // Act
        product.DecreaseStock(3);

        // Assert
        Assert.Equal(7, product.Stock);
    }
}