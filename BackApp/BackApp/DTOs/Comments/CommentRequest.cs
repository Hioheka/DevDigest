using System.ComponentModel.DataAnnotations;

namespace BackApp.DTOs.Comments;

public class CommentRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;
}
