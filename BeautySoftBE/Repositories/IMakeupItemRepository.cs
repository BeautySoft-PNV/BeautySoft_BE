using BeautySoftBE.Models;

namespace BeautySoftBE.Repositories;

public interface IMakeupItemRepository
{
    Task<IEnumerable<MakeupItemModel>> GetAllAsync();
    Task<MakeupItemModel> GetByIdAsync(int id);
    Task<MakeupItemModel> CreateAsync(MakeupItemModel makeupItem);
    Task<bool> UpdateAsync(MakeupItemModel makeupItem);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<MakeupItemModel>> GetByUserIdAsync(int userId);
}