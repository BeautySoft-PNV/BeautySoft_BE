using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services
{
    public class MakeupItemService : IMakeupItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly FirebaseStorageService _firebaseStorageService;

        public MakeupItemService(ApplicationDbContext context, FirebaseStorageService firebaseStorageService)
        {
            _context = context;
            _firebaseStorageService = firebaseStorageService;
        }

        public async Task<IEnumerable<MakeupItemModel>> GetAllAsync()
        {
            return await _context.MakeupItems
                .Include(m => m.User)
                .Include(m => m.MakeupItemStyles)
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
                throw new Exception("UserId không tồn tại trong hệ thống.");
            }
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imageUrl = await _firebaseStorageService.UploadImageAsync(stream, imageFile.FileName);
                makeupItem.Image = imageUrl;
            }

            _context.MakeupItems.Add(makeupItem);
            await _context.SaveChangesAsync();
            return makeupItem;
        }

        public async Task<bool> UpdateAsync(MakeupItemModel makeupItem, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imageUrl = await _firebaseStorageService.UploadImageAsync(stream, imageFile.FileName);
                makeupItem.Image = imageUrl;
            }
            _context.Entry(makeupItem).State = EntityState.Modified;
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
