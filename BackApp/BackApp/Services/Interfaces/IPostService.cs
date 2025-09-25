using BackApp.DTOs.Posts;

namespace BackApp.Services.Interfaces;

public interface IPostService
{
    Task<IEnumerable<PostResponse>> GetAllAsync();
    Task<IEnumerable<PostResponse>> GetByAuthorAsync(string authorId);
    Task<PostResponse?> GetByIdAsync(int id);
    Task<PostResponse> CreateAsync(string authorId, PostRequest request);
    Task<PostResponse?> UpdateAsync(int id, string authorId, PostRequest request, bool isAdmin);
    Task<bool> DeleteAsync(int id, string authorId, bool isAdmin);
}
