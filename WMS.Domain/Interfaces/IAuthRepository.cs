using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAuthRepository
{
    Task<UserLogin?> GetByUsernameAsync(string username);
    Task<UserLogin?> GetByIdAsync(int userId);
    Task UpdateLastLoginAsync(int userId);
    Task UpdateUserAsync(UserLogin user);
}
