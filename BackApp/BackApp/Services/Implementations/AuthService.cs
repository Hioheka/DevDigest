using BackApp.DTOs.Auth;
using BackApp.Entities;
using BackApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackApp.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string role)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already registered.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        await _userManager.AddToRoleAsync(user, role);

        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, expiresAt) = await _tokenService.GenerateAccessTokenAsync(user, roles);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Role = roles.First()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, expiresAt) = await _tokenService.GenerateAccessTokenAsync(user, roles);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Role = roles.FirstOrDefault() ?? string.Empty
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var existingToken = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (existingToken is null || existingToken.User is null)
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }

        await _tokenService.RevokeRefreshTokenAsync(existingToken);

        var user = existingToken.User;
        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, expiresAt) = await _tokenService.GenerateAccessTokenAsync(user, roles);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Role = roles.FirstOrDefault() ?? string.Empty
        };
    }
}
