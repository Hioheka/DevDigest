using BackApp.DTOs.Comments;
using BackApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackApp.Controllers;

[ApiController]
[Route("api/posts/{postId:int}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> Get(int postId)
    {
        var comments = await _commentService.GetByPostAsync(postId);
        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentResponse>> Create(int postId, [FromBody] CommentRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var comment = await _commentService.CreateAsync(postId, userId, request);
        return Ok(comment);
    }

    [HttpDelete("{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int postId, int commentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        try
        {
            var deleted = await _commentService.DeleteAsync(commentId, userId, isAdmin);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
