using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Repositories;

public class MakeupItemRepository : IMakeupItemRepository
{
    private readonly ApplicationDbContext _context;

    public MakeupItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MakeupItemModel>> GetAllAsync()
    {
        return await _context.MakeupItems.Include(m => m.User).Include(m => m.MakeupItemStyles).ToListAsync();
    }

    public async Task<MakeupItemModel> GetByIdAsync(int id)
    {
        return await _context.MakeupItems.Include(m => m.User).Include(m => m.MakeupItemStyles).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MakeupItemModel> CreateAsync(MakeupItemModel makeupItem)
    {
        _context.MakeupItems.Add(makeupItem);
        await _context.SaveChangesAsync();
        return makeupItem;
    }

    public async Task<bool> UpdateAsync(MakeupItemModel makeupItem)
    {
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
}