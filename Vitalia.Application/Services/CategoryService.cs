using Vitalia.Application.DTOs.Category;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Domain.Entities;

namespace Vitalia.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories.Select(MapToResponse);
    }

    public async Task<CategoryResponse?> GetByIdAsync(long id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        return category is null
            ? null
            : MapToResponse(category);
    }

    public async Task<CategoryResponse> AddAsync(CategoryRequest request)
    {
        var category = new Category(
            request.Name,
            request.Description
        );

        await _categoryRepository.AddAsync(category);

        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(category);
    }

    public async Task UpdateAsync(long id, CategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            throw new KeyNotFoundException(
                $"Categoria com ID {id} não encontrada."
            );
        }

        category.Update(
            request.Name,
            request.Description
        );

        _categoryRepository.Update(category);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            throw new KeyNotFoundException(
                $"Categoria com ID {id} não encontrada."
            );
        }

        _categoryRepository.Delete(category);

        await _unitOfWork.SaveChangesAsync();
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}