using BeautySoftBE.Application.DTOs;
using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;
using Supabase;

namespace BeautySoftBE.Services
{
    public class MakeupItemService : IMakeupItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly Client _supabase;
        public MakeupItemService(ApplicationDbContext context, Client supabase)
        {
            _context = context;
            _supabase = supabase;
        }

        public async Task<IEnumerable<MakeupItemModel>> GetAllAsync()
        {
            return await _context.MakeupItems
                .Include(m => m.User)
                .Include(m => m.MakeupItemStyles)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<MakeupItemModel> GetByIdAsync(int id)
        {
            return await _context.MakeupItems
                .Include(m => m.User)
                .Include(m => m.MakeupItemStyles)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MakeupItemModel> CreateAsync(MakeupItemModel makeupItem, IFormFile imageFile)
        {
            if (makeupItem == null)
            {
                throw new ArgumentNullException(nameof(makeupItem), "Dữ liệu không hợp lệ.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == makeupItem.UserId);
            if (!userExists)
            {
                throw new Exception("UserId does not exist in the system.");
            }
            
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    string filePath = uniqueFileName;

                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        var storage = _supabase.Storage.From("sinh");
                        await storage.Upload(fileBytes, filePath);
                        var publicUrl = storage.GetPublicUrl(filePath);
                        makeupItem.Image = publicUrl;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving image to Supabase Storage", ex);
                }
            }

            _context.MakeupItems.Add(makeupItem);
            await _context.SaveChangesAsync();
            return makeupItem;
        }


        public async Task<bool> UpdateAsync(string id, MakeupItemRequestDTO makeupItem, IFormFile imageFile)
        {
            int numericId = int.Parse(id);
            var existingItem = await _context.MakeupItems.FindAsync(numericId);
            if (existingItem == null)
            {
                throw new KeyNotFoundException("No makeup products found.");
            }

            existingItem.Name = makeupItem.Name;
            existingItem.Description = makeupItem.Description;
    
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    string filePath = uniqueFileName;

                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        var storage = _supabase.Storage.From("sinh");
                        await storage.Upload(fileBytes, filePath);
                        var publicUrl = storage.GetPublicUrl(filePath);
                
                        if (!string.IsNullOrEmpty(existingItem.Image))
                        {
                            try
                            {
                                await storage.Remove(new List<string> { Path.GetFileName(existingItem.Image) });
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Error deleting old image from Supabase Storage", ex);
                            }
                        }
                
                        existingItem.Image = publicUrl;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating image on Supabase Storage", ex);
                }
            }

            _context.Entry(existingItem).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var makeupItem = await _context.MakeupItems.FindAsync(id);
            if (makeupItem == null)
            {
                return false;
            }

            _context.MakeupItems.Remove(makeupItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MakeupItemModel>> GetByUserIdAsync(int userId)
        {
            return await _context.MakeupItems
                .Where(item => item.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MakeupItemModel>> SearchByNameAsync(string? name)
        {
            return await _context.MakeupItems
                .Where(m => string.IsNullOrEmpty(name) || m.Name.Contains(name))
                .ToListAsync();
        }
    }
}
