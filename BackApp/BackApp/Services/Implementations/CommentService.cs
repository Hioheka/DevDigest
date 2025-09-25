using BackApp.DataAccess.Repositories;
using BackApp.DTOs.Comments;
using BackApp.Entities;
using BackApp.Services.Interfaces;

namespace BackApp.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CommentResponse>> GetByPostAsync(int postId)
    {
        var comments = await _unitOfWork.Comments.GetAllAsync(c => c.BlogPostId == postId, c => c.Author!);
        return comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            Content = c.Content,
            BlogPostId = c.BlogPostId,
            AuthorId = c.AuthorId,
            AuthorEmail = c.Author?.Email ?? string.Empty,
            CreatedAt = c.CreatedAt
        });
    }

    public async Task<CommentResponse> CreateAsync(int postId, string authorId, CommentRequest request)
    {
        var comment = new Comment
        {
            Content = request.Content,
            BlogPostId = postId,
            AuthorId = authorId
        };

        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.CommitAsync();

        var saved = await _unitOfWork.Comments.GetByIdAsync(comment.Id, c => c.Author!);
        return new CommentResponse
        {
            Id = saved!.Id,
            Content = saved.Content,
            BlogPostId = saved.BlogPostId,
            AuthorId = saved.AuthorId,
            AuthorEmail = saved.Author?.Email ?? string.Empty,
            CreatedAt = saved.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int commentId, string requesterId, bool isAdmin)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment is null)
        {
            return false;
        }

        if (!isAdmin && comment.AuthorId != requesterId)
        {
            throw new UnauthorizedAccessException("You can only delete your own comments.");
        }

        _unitOfWork.Comments.Remove(comment);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
