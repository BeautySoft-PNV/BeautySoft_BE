using BeautySoftBE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySoftBE.Repositories
{
    public interface IUserRepository
    {
        Task<UserModel?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(UserModel user);
        Task<bool> DeleteAsync(int id);
        Task AddUserAsync(UserModel user);
        string GetRoleNameById(int roleId);
        Task<bool> UserExistsAsync(string email);
        Task<UserModel?> GetEmailByUsernameAsync(string email);
    }
}