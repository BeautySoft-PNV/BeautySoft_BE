using BeautySoftBE.Models;
using System.Threading.Tasks;
using BeautySoftBE.Application.DTOs;

namespace BeautySoftBE.Services
{
    public interface IUserService
    {
        Task<UserModel?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(UserRequestDTO user,String newPassword, IFormFile? imageFile);
        Task<bool> DeleteAsync(int id);
        Task<List<UserModel>> GetAllAsync();
        Task<bool> BlockUserAsync(int userId);
        Task<bool> UnblockUserAsync(int userId);
    }
}