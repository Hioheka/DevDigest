namespace BackApp.Entities;

public class BlogPost : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string AuthorId { get; set; } = string.Empty;

    public Category? Category { get; set; }
    public ApplicationUser? Author { get; set; }
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
}
