using BackApp.DTOs.Comments;

namespace BackApp.Services.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentResponse>> GetByPostAsync(int postId);
    Task<CommentResponse> CreateAsync(int postId, string authorId, CommentRequest request);
    Task<bool> DeleteAsync(int commentId, string requesterId, bool isAdmin);
}
