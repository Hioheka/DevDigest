using System.ComponentModel.DataAnnotations;

namespace BackApp.DTOs.Categories;

public class CategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
