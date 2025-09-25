using BackApp.DTOs.Categories;

namespace BackApp.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse?> GetByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CategoryRequest request);
    Task<CategoryResponse?> UpdateAsync(int id, CategoryRequest request);
    Task<bool> DeleteAsync(int id);
}
