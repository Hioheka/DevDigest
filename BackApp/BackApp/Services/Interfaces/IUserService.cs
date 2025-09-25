using BackApp.DTOs.Users;

namespace BackApp.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
}
