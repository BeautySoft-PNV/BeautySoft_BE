using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services;

public class MakeupStyleService : IMakeupStyleService
{
    private readonly ApplicationDbContext _context;
    private readonly FirebaseStorageService _firebaseStorageService;

    public MakeupStyleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MakeupStyleModel>> GetAllAsync()
    {
        return await _context.MakeupStyles.ToListAsync();
    }

    public async Task<MakeupStyleModel> GetByIdAsync(int id)
    {
        return await _context.MakeupStyles.FindAsync(id);
    }

    public async Task CreateAsync(MakeupStyleModel makeupStyle, IFormFile imageFile)
    {
        if (makeupStyle == null)
        {
            throw new ArgumentNullException(nameof(makeupStyle), "Dữ liệu không hợp lệ.");
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            var imageUrl = await _firebaseStorageService.UploadImageAsync(stream, imageFile.FileName);
            makeupStyle.Image = imageUrl;
        }
        
        await _context.MakeupStyles.AddAsync(makeupStyle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MakeupStyleModel makeupStyle, IFormFile imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            var imageUrl = await _firebaseStorageService.UploadImageAsync(stream, imageFile.FileName);
            makeupStyle.Image = imageUrl;
        }
        _context.MakeupStyles.Update(makeupStyle);
        await _context.SaveChangesAsync();
    }

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