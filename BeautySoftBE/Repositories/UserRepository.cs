using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BeautySoftBE.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(UserModel user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null) return false;

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Avatar = user.Avatar;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<UserModel?> GetEmailByUsernameAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Name == username);
        }

        public async Task AddUserAsync(UserModel user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    
        public string GetRoleNameById(int roleId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
            return role?.Name ?? "User";
        }
    }
}