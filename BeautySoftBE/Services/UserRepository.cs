using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BeautySoftBE.Services;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
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