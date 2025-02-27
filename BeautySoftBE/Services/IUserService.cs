using BeautySoftBE.Models;
using System.Threading.Tasks;

namespace BeautySoftBE.Services
{
    public interface IUserService
    {
        Task<UserModel?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(UserModel user, IFormFile imageFile);
        Task<bool> DeleteAsync(int id);
    }
}