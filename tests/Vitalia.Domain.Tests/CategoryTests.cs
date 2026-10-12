using Vitalia.Domain.Entities;

namespace Vitalia.Domain.Tests;

public class CategoryTests
{
    [Fact]
    public void CriarCategory_ComDadosValidos_DeveCriarCategoria()
    {
        // Arrange
        const string name = "Alimentação";
        const string description = "Produtos alimentícios";

        // Act
        var category = new Category(
            name,
            description);

        // Assert
        Assert.Equal(name, category.Name);
        Assert.Equal(description, category.Description);
    }

    [Fact]
    public void AtualizarCategory_DeveAlterarDados()
    {
        // Arrange
        var category = new Category(
            "Alimentação",
            "Descrição inicial");

        // Act
        category.Update(
            "Higiene",
            "Produtos de higiene");

        // Assert
        Assert.Equal("Higiene", category.Name);
        Assert.Equal(
            "Produtos de higiene",
            category.Description);
    }
}