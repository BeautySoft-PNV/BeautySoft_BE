using System.Security.Cryptography;
using System.Text;
using BeautySoftBE.Models;
using BeautySoftBE.Repositories;
using System.Threading.Tasks;
using BeautySoftBE.Application.DTOs;

namespace BeautySoftBE.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                
                if (!string.IsNullOrEmpty(existingUser.Avatar))
                {
                    string oldFilePath = Path.Combine(uploadsFolder, Path.GetFileName(existingUser.Avatar));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                existingUser.Avatar = $"/uploads/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving image to server", ex);
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
    }
}