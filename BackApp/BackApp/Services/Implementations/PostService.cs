using BackApp.DataAccess.Repositories;
using BackApp.DTOs.Posts;
using BackApp.Entities;
using BackApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackApp.Services.Implementations;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;

    public PostService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PostResponse>> GetAllAsync()
    {
        var posts = await _unitOfWork.BlogPosts.GetAllAsync(null, p => p.Category!, p => p.Author!);
        return posts.Select(MapToResponse);
    }

    public async Task<IEnumerable<PostResponse>> GetByAuthorAsync(string authorId)
    {
        var posts = await _unitOfWork.BlogPosts.GetAllAsync(p => p.AuthorId == authorId, p => p.Category!, p => p.Author!);
        return posts.Select(MapToResponse);
    }

    public async Task<PostResponse?> GetByIdAsync(int id)
    {
        var post = await _unitOfWork.BlogPosts.GetByIdAsync(id, p => p.Category!, p => p.Author!);
        return post is null ? null : MapToResponse(post);
    }

    public async Task<PostResponse> CreateAsync(string authorId, PostRequest request)
    {
        var post = new BlogPost
        {
            Title = request.Title,
            Content = request.Content,
            CategoryId = request.CategoryId,
            CoverImageUrl = request.CoverImageUrl,
            AuthorId = authorId
        };

        await _unitOfWork.BlogPosts.AddAsync(post);
        await _unitOfWork.CommitAsync();

        var created = await _unitOfWork.BlogPosts.GetByIdAsync(post.Id, p => p.Category!, p => p.Author!);
        return MapToResponse(created!);
    }

    public async Task<PostResponse?> UpdateAsync(int id, string authorId, PostRequest request, bool isAdmin)
    {
        var post = await _unitOfWork.BlogPosts.GetByIdAsync(id);
        if (post is null)
        {
            return null;
        }

        if (!isAdmin && post.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("You can only modify your own posts.");
        }

        post.Title = request.Title;
        post.Content = request.Content;
        post.CategoryId = request.CategoryId;
        post.CoverImageUrl = request.CoverImageUrl;

        _unitOfWork.BlogPosts.Update(post);
        await _unitOfWork.CommitAsync();

        var updated = await _unitOfWork.BlogPosts.GetByIdAsync(post.Id, p => p.Category!, p => p.Author!);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id, string authorId, bool isAdmin)
    {
        var post = await _unitOfWork.BlogPosts.GetByIdAsync(id);
        if (post is null)
        {
            return false;
        }

        if (!isAdmin && post.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("You can only delete your own posts.");
        }

        _unitOfWork.BlogPosts.Remove(post);
        await _unitOfWork.CommitAsync();
        return true;
    }

    private static PostResponse MapToResponse(BlogPost post)
    {
        return new PostResponse
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CoverImageUrl = post.CoverImageUrl,
            CategoryId = post.CategoryId,
            CategoryName = post.Category?.Name ?? string.Empty,
            AuthorId = post.AuthorId,
            AuthorEmail = post.Author?.Email ?? string.Empty,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
