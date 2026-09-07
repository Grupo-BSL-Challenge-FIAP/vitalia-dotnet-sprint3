using Moq;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Services;
using Vitalia.Domain.Entities;
using Vitalia.Application.DTOs.Product;

namespace Vitalia.Application.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task BuscarProduto_IdInexistente_DeveRetornarNull()
    {
        // Arrange
        var productRepositoryMock =
            new Mock<IProductRepository>();

        var categoryRepositoryMock =
            new Mock<ICategoryRepository>();

        var unitOfWorkMock =
            new Mock<IUnitOfWork>();

        const long productId = 999;

        productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        var result = await service.GetByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task AdicionarProduto_CategoriaInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var productRepositoryMock =
            new Mock<IProductRepository>();

        var categoryRepositoryMock =
            new Mock<ICategoryRepository>();

        var unitOfWorkMock =
            new Mock<IUnitOfWork>();

        var request = new ProductRequest
        {
            CategoryId = 999,
            Name = "Produto Teste",
            Description = "Produto para teste",
            Price = 50,
            Stock = 10
        };

        categoryRepositoryMock
            .Setup(repository =>
                repository.ExistsAsync(request.CategoryId))
            .ReturnsAsync(false);

        var service = new ProductService(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AddAsync(request)
        );

        // Assert
        Assert.Equal(
            $"Categoria com ID {request.CategoryId} não encontrada.",
            exception.Message
        );
    }
    
    [Fact]
    public async Task AdicionarProduto_CategoriaExistente_DeveCadastrarProduto()
    {
        // Arrange
        var productRepositoryMock =
            new Mock<IProductRepository>();

        var categoryRepositoryMock =
            new Mock<ICategoryRepository>();

        var unitOfWorkMock =
            new Mock<IUnitOfWork>();

        var request = new ProductRequest
        {
            CategoryId = 1,
            Name = "Ração Premium",
            Description = "Ração premium para cães",
            Price = 149.90m,
            Stock = 20
        };

        categoryRepositoryMock
            .Setup(repository =>
                repository.ExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        productRepositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var service = new ProductService(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        var result = await service.AddAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.CategoryId, result.CategoryId);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Price, result.Price);
        Assert.Equal(request.Stock, result.Stock);
        Assert.Equal("ACTIVE", result.Status);

        productRepositoryMock.Verify(
            repository =>
                repository.AddAsync(It.IsAny<Product>()),
            Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }
    
    [Fact]
    public async Task AtualizarProduto_ProdutoInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var productRepositoryMock =
            new Mock<IProductRepository>();

        var categoryRepositoryMock =
            new Mock<ICategoryRepository>();

        var unitOfWorkMock =
            new Mock<IUnitOfWork>();

        const long productId = 999;

        var request = new ProductRequest
        {
            CategoryId = 1,
            Name = "Produto Atualizado",
            Description = "Descrição atualizada",
            Price = 99.90m,
            Stock = 15
        };

        // A categoria existe, então o fluxo pode continuar
        categoryRepositoryMock
            .Setup(repository =>
                repository.ExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        // Mas o produto não existe
        productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.UpdateAsync(productId, request)
            );

        // Assert
        Assert.Equal(
            $"Produto com ID {productId} não encontrado.",
            exception.Message
        );

        productRepositoryMock.Verify(
            repository =>
                repository.Update(It.IsAny<Product>()),
            Times.Never
        );

        unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never
        );
    }
    
    [Fact]
    public async Task AtualizarProduto_ProdutoExistente_DeveAtualizarProduto()
    {
        // Arrange
        var productRepositoryMock =
            new Mock<IProductRepository>();

        var categoryRepositoryMock =
            new Mock<ICategoryRepository>();

        var unitOfWorkMock =
            new Mock<IUnitOfWork>();

        const long productId = 1;

        var product = new Product(
            1,
            "Produto Antigo",
            "Descrição antiga",
            50m,
            10
        );

        var request = new ProductRequest
        {
            CategoryId = 2,
            Name = "Produto Atualizado",
            Description = "Descrição atualizada",
            Price = 99.90m,
            Stock = 25
        };

        categoryRepositoryMock
            .Setup(repository =>
                repository.ExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync(product);

        unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var service = new ProductService(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        await service.UpdateAsync(productId, request);

        // Assert
        Assert.Equal(request.CategoryId, product.CategoryId);
        Assert.Equal(request.Name, product.Name);
        Assert.Equal(request.Description, product.Description);
        Assert.Equal(request.Price, product.Price);
        Assert.Equal(request.Stock, product.Stock);

        productRepositoryMock.Verify(
            repository =>
                repository.Update(product),
            Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }
    
    [Fact]
    public async Task ExcluirProduto_ProdutoInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var productRepositoryMock =
            new Mock<IProductRepository>();

        var categoryRepositoryMock =
            new Mock<ICategoryRepository>();

        var unitOfWorkMock =
            new Mock<IUnitOfWork>();

        const long productId = 999;

        productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.DeleteAsync(productId)
            );

        // Assert
        Assert.Equal(
            $"Produto com ID {productId} não encontrado.",
            exception.Message
        );

        productRepositoryMock.Verify(
            repository =>
                repository.Delete(It.IsAny<Product>()),
            Times.Never
        );

        unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never
        );
    }
    
    [Fact]
    public async Task ExcluirProduto_ProdutoExistente_DeveExcluirProduto()
    {
        // Arrange
        var productRepositoryMock =
            new Mock<IProductRepository>();

        var categoryRepositoryMock =
            new Mock<ICategoryRepository>();

        var unitOfWorkMock =
            new Mock<IUnitOfWork>();

        const long productId = 1;

        var product = new Product(
            1,
            "Produto Teste",
            "Produto para exclusão",
            50m,
            10
        );

        productRepositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(productId))
            .ReturnsAsync(product);

        unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var service = new ProductService(
            productRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        await service.DeleteAsync(productId);

        // Assert
        productRepositoryMock.Verify(
            repository =>
                repository.Delete(product),
            Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once
        );
    }
}