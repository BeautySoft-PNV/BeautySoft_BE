using System.Security.Cryptography;
using System.Text;
using BeautySoftBE.Models;
using BeautySoftBE.Repositories;
using System.Threading.Tasks;
using BeautySoftBE.Application.DTOs;
using BeautySoftBE.Data;
using Microsoft.EntityFrameworkCore;
using Supabase;

namespace BeautySoftBE.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly Client _supabase;
        private readonly ApplicationDbContext _context;
        public UserService(IUserRepository userRepository,  Client supabase, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _supabase = supabase;
            _context = context;
        }

        public async Task<UserModel?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

       public async Task<bool> UpdateAsync(UserRequestDTO user, string newPassword, IFormFile? imageFile)
{
    if (user == null)
    {
        throw new ArgumentNullException(nameof(user), "User data is required");
    }
    if (user.Id <= 0)
    {
        throw new ArgumentException("Invalid user ID", nameof(user.Id));
    }

    var existingUser = await _userRepository.GetByIdAsync(user.Id);
    if (existingUser == null)
    {
        throw new KeyNotFoundException($"User with ID {user.Id} not found");
    }

    existingUser.Name = user.Name;
    existingUser.Email = user.Email;
    existingUser.RoleId = user.RoleId;

    if (imageFile != null && imageFile.Length > 0 && imageFile.ContentType != "text/html")
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
                
                if (!string.IsNullOrEmpty(existingUser.Avatar))
                {
                    try
                    {
                        await storage.Remove(new List<string> { Path.GetFileName(existingUser.Avatar) });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error deleting old image from Supabase Storage", ex);
                    }
                }
                
                existingUser.Avatar = publicUrl;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error saving image to Supabase Storage", ex);
        }
    }

    if (!string.IsNullOrEmpty(newPassword))
    {
        existingUser.Password = HashPassword(newPassword);
    }

    return await _userRepository.UpdateAsync(existingUser);
}

       
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }
        
        public async Task<List<UserModel>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        
        public async Task<bool> BlockUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsBlocked) return false; 

            user.IsBlocked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.IsBlocked) return false; 
            user.IsBlocked = false;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}