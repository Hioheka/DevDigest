using BackApp.Entities;

namespace BackApp.Services.Interfaces;

public interface ITokenService
{
    Task<(string AccessToken, DateTime ExpiresAt)> GenerateAccessTokenAsync(ApplicationUser user, IList<string> roles);
    Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(RefreshToken token);
}
