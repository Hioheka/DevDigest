using Microsoft.AspNetCore.Identity;

namespace BackApp.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
}
