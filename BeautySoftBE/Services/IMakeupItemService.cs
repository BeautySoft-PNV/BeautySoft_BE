﻿using BeautySoftBE.Models;

namespace BeautySoftBE.Services;

public interface IMakeupItemService
{
    Task<IEnumerable<MakeupItemModel>> GetAllAsync();
    Task<MakeupItemModel> GetByIdAsync(int id);
    Task<MakeupItemModel> CreateAsync(MakeupItemModel makeupItem, IFormFile imageFile);
    Task<bool> UpdateAsync(string id, MakeupItemModel makeupItem, IFormFile imageFile);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<MakeupItemModel>> GetByUserIdAsync(int userId);
    Task<IEnumerable<MakeupItemModel>> SearchByNameAsync(string? name);

}