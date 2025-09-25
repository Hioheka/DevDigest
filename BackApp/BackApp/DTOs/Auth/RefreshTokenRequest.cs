using System.ComponentModel.DataAnnotations;

namespace BackApp.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
