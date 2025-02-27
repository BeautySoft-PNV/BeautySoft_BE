using BeautySoftBE.Models;

namespace BeautySoftBE.Services;

public interface IMakeupStyleService
{
    Task<IEnumerable<MakeupStyleModel>> GetAllAsync();
    Task<MakeupStyleModel> GetByIdAsync(int id);
    Task CreateAsync(MakeupStyleModel makeupStyle, IFormFile imageFile);
    Task UpdateAsync(MakeupStyleModel makeupStyle, IFormFile imageFile);
    Task DeleteAsync(int id);
    Task<IEnumerable<MakeupStyleModel>> GetByUserIdAsync(int userId);
}