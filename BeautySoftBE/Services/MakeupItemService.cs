using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services
{
    public class MakeupItemService : IMakeupItemService
    {
        private readonly ApplicationDbContext _context;

        public MakeupItemService(ApplicationDbContext context)
        {
            _context = context;
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

            // Lưu ảnh vào thư mục trong source
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Lưu đường dẫn ảnh vào database
                    makeupItem.Image = $"/uploads/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi lưu ảnh vào server", ex);
                }
            }

            _context.MakeupItems.Add(makeupItem);
            await _context.SaveChangesAsync();
            return makeupItem;
        }

        public async Task<bool> UpdateAsync(MakeupItemModel makeupItem, IFormFile imageFile)
        {
            var existingItem = await _context.MakeupItems.FindAsync(makeupItem.Id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException("Không tìm thấy sản phẩm trang điểm.");
            }

            existingItem.Name = makeupItem.Name;
            existingItem.Description = makeupItem.Description;

            // Lưu ảnh vào thư mục trong source
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Xóa ảnh cũ (nếu có)
                    if (!string.IsNullOrEmpty(existingItem.Image))
                    {
                        string oldFilePath = Path.Combine(uploadsFolder, Path.GetFileName(existingItem.Image));
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                        }
                    }

                    // Cập nhật đường dẫn ảnh mới
                    existingItem.Image = $"/uploads/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi cập nhật ảnh trên server", ex);
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
