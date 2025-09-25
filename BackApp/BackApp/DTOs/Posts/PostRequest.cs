using System.ComponentModel.DataAnnotations;

namespace BackApp.DTOs.Posts;

public class PostRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    public string? CoverImageUrl { get; set; }
}
