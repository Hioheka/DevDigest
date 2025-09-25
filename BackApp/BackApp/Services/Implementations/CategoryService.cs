using BackApp.DataAccess.Repositories;
using BackApp.DTOs.Categories;
using BackApp.Entities;
using BackApp.Services.Interfaces;

namespace BackApp.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(MapToResponse);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category is null ? null : MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.CommitAsync();

        return MapToResponse(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(int id, CategoryRequest request)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category is null)
        {
            return null;
        }

        category.Name = request.Name;
        category.Description = request.Description;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.CommitAsync();

        return MapToResponse(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category is null)
        {
            return false;
        }

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.CommitAsync();
        return true;
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
