
using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;
using Supabase;


namespace BeautySoftBE.Services;

public class MakeupStyleService : IMakeupStyleService
{
    private readonly ApplicationDbContext _context;
    private readonly Client _supabase;

    public MakeupStyleService(ApplicationDbContext context, Client supabase)
    {
        _context = context;
        _supabase = supabase;
    }

    public async Task<IEnumerable<MakeupStyleModel>> GetAllAsync()
    {
        return await _context.MakeupStyles
            .OrderByDescending(m => m.Id)
            .ToListAsync();
    }


    public async Task<MakeupStyleModel> GetByIdAsync(int id)
    {
        return await _context.MakeupStyles.FindAsync(id);
    }

    public async Task CreateAsync(MakeupStyleModel makeupStyle, IFormFile imageFile)
    {
        if (makeupStyle == null)
        {
            throw new ArgumentNullException(nameof(makeupStyle),"Invalid data.");
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

                    if (!string.IsNullOrEmpty(makeupStyle.Image))
                    {
                        try
                        {
                            await storage.Remove(new List<string> { Path.GetFileName(makeupStyle.Image) });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error deleting old image from Supabase Storage", ex);
                        }
                    }

                    makeupStyle.Image = publicUrl;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving image to server", ex);
            }
        }

        await _context.MakeupStyles.AddAsync(makeupStyle);
        await _context.SaveChangesAsync();
    }

    /*public async Task UpdateAsync(MakeupStyleModel makeupStyle)
    {
        var existingStyle = await _context.MakeupStyles.FindAsync(makeupStyle.Id);
        if (existingStyle == null)
        {
            throw new KeyNotFoundException("Không tìm thấy phong cách trang điểm.");
        }
        
        existingStyle.Guidance = makeupStyle.Guidance;
        
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
                
                if (!string.IsNullOrEmpty(existingStyle.Image))
                {
                    string oldFilePath = Path.Combine(uploadsFolder, Path.GetFileName(existingStyle.Image));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Cập nhật đường dẫn ảnh mới
                existingStyle.Image = $"/uploads/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật ảnh trên server", ex);
            }
        }

        _context.MakeupStyles.Update(existingStyle);
        await _context.SaveChangesAsync();
    }*/

    public async Task DeleteAsync(int id)
    {
        var makeupStyle = await _context.MakeupStyles.FindAsync(id);
        if (makeupStyle != null)
        {
            _context.MakeupStyles.Remove(makeupStyle);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<MakeupStyleModel>> GetByUserIdAsync(int userId)
    {
        return await _context.MakeupStyles
            .Where(ms => ms.UserId == userId)
            .ToListAsync();
    }

}