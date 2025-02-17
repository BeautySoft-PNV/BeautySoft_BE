using BeautySoftBE.Models;
using BeautySoftBE.Repositories;

namespace BeautySoftBE.Services;

public class MakeupItemService : IMakeupItemService
{
    private readonly IMakeupItemRepository _repository;

    public MakeupItemService(IMakeupItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MakeupItemModel>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<MakeupItemModel> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    public async Task<MakeupItemModel> CreateAsync(MakeupItemModel makeupItem) => await _repository.CreateAsync(makeupItem);
    public async Task<bool> UpdateAsync(MakeupItemModel makeupItem) => await _repository.UpdateAsync(makeupItem);
    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
}