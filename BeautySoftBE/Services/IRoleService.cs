using BeautySoftBE.Models;

namespace BeautySoftBE.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetAllAsync();
        Task<RoleModel?> GetByIdAsync(int id);
        Task<RoleModel> CreateAsync(RoleModel role);
        Task<bool> UpdateAsync(RoleModel role);
        Task<bool> DeleteAsync(int id);
    }
}