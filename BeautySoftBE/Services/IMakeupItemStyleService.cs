using BeautySoftBE.Models;

namespace BeautySoftBE.Services;

public interface IMakeupItemStyleService
{
    Task<bool> ValidateMakeupItemAndStyleAsync(int makeupItemId, int makeupStyleId);
    Task<IEnumerable<MakeupItemStyleModel>> GetByUserIdAsync(int userId);
    Task CreateAsync(MakeupItemStyleModel makeupItemStyle);
    Task<bool> ValidateUserAsync(int userId);
}