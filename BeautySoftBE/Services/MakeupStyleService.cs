using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services;

public class MakeupStyleService : IMakeupStyleService
{
    private readonly ApplicationDbContext _context;

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

    public async Task CreateAsync(MakeupStyleModel makeupStyle)
    {
        await _context.MakeupStyles.AddAsync(makeupStyle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MakeupStyleModel makeupStyle)
    {
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
}