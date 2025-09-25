namespace BackApp.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<BlogPost> Posts { get; set; } = new HashSet<BlogPost>();
}
