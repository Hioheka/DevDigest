namespace BackApp.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public int BlogPostId { get; set; }
    public string AuthorId { get; set; } = string.Empty;

    public BlogPost? BlogPost { get; set; }
    public ApplicationUser? Author { get; set; }
}
