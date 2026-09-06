using Vitalia.Application.DTOs.Product;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Domain.Entities;

namespace Vitalia.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(MapToResponse);
    }

    public async Task<ProductResponse?> GetByIdAsync(long id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        return product is null
            ? null
            : MapToResponse(product);
    }

    public async Task<ProductResponse> AddAsync(ProductRequest request)
    {
        var categoryExists =
            await _categoryRepository.ExistsAsync(request.CategoryId);

        if (!categoryExists)
        {
            throw new KeyNotFoundException(
                $"Categoria com ID {request.CategoryId} não encontrada."
            );
        }

        var product = new Product(
            request.CategoryId,
            request.Name,
            request.Description,
            request.Price,
            request.Stock
        );

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task UpdateAsync(long id, ProductRequest request)
    {
        var categoryExists =
            await _categoryRepository.ExistsAsync(request.CategoryId);

        if (!categoryExists)
        {
            throw new KeyNotFoundException(
                $"Categoria com ID {request.CategoryId} não encontrada."
            );
        }

        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Produto com ID {id} não encontrado."
            );
        }

        product.Update(
            request.CategoryId,
            request.Name,
            request.Description,
            request.Price,
            request.Stock
        );

        _productRepository.Update(product);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Produto com ID {id} não encontrado."
            );
        }

        _productRepository.Delete(product);

        await _unitOfWork.SaveChangesAsync();
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}