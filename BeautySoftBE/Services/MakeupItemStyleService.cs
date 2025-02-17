using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services
{
    public class MakeupItemStyleService : IMakeupItemStyleService
    {
        private readonly ApplicationDbContext _context;

        public MakeupItemStyleService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<bool> ValidateMakeupItemAndStyleAsync(int makeupItemId, int makeupStyleId)
        {
            bool isMakeupItemExists = await _context.MakeupItems.AnyAsync(m => m.Id == makeupItemId);
            bool isMakeupStyleExists = await _context.MakeupStyles.AnyAsync(s => s.Id == makeupStyleId);
    
            return isMakeupItemExists && isMakeupStyleExists;
        }
        
        public async Task<IEnumerable<MakeupItemStyleModel>> GetByUserIdAsync(int userId)
        {
            var query = _context.MakeupItemStyles
                .Include(mis => mis.MakeupItem)  
                .Include(mis => mis.MakeupStyle) 
                .Where(mis => mis.MakeupItem.UserId == userId || mis.MakeupStyle.UserId == userId);
            
            return await query
                .Select(mis => new MakeupItemStyleModel
                {
                    Id = mis.Id,
                    MakeupItemId = mis.MakeupItem.Id,          
                    MakeupStyleId = mis.MakeupStyle.Id,       
                    UserId = mis.MakeupItem.UserId ,
                    MakeupItem = mis.MakeupItem,            
                    MakeupStyle = mis.MakeupStyle           
                })
                .ToListAsync();
        }
        
        public async Task CreateAsync(MakeupItemStyleModel makeupItemStyle)
        {
            var newMakeupItemStyle = new MakeupItemStyleModel
            {
                MakeupItemId = makeupItemStyle.MakeupItemId,
                MakeupStyleId = makeupItemStyle.MakeupStyleId,
                UserId = makeupItemStyle.UserId
            };
            
            await _context.MakeupItemStyles.AddAsync(newMakeupItemStyle);
            await _context.SaveChangesAsync();
        }

        
        public async Task<bool> ValidateUserAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

    }
}