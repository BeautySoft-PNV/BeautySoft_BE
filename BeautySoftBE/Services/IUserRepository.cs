using BeautySoftBE.Models;

namespace BeautySoftBE.Services;

public interface IUserRepository
{
    Task<UserModel?> GetEmailByUsernameAsync(string email);
    Task<bool> UserExistsAsync(string username);
    Task AddUserAsync(UserModel user);
    string GetRoleNameById(int roleId);
}