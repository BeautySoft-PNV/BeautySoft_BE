using BeautySoftBE.Data;
using BeautySoftBE.Models;
using BeautySoftBE.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services;

public class MakeupItemService : IMakeupItemService
{
    private readonly IMakeupItemRepository _repository;
    private readonly ApplicationDbContext _context;


    public MakeupItemService(IMakeupItemRepository repository, ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }
    public async Task<IEnumerable<MakeupItemModel>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }
    public async Task<IEnumerable<MakeupItemModel>> SearchByNameAsync(string? name)
    {
        return await _context.MakeupItems
            .Where(m => string.IsNullOrEmpty(name) || m.Name.Contains(name))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MakeupItemModel>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<MakeupItemModel> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    public async Task<MakeupItemModel> CreateAsync(MakeupItemModel makeupItem) => await _repository.CreateAsync(makeupItem);
    public async Task<bool> UpdateAsync(MakeupItemModel makeupItem) => await _repository.UpdateAsync(makeupItem);
    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
    
}