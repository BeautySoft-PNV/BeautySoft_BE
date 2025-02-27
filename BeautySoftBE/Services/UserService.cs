using BeautySoftBE.Models;
using BeautySoftBE.Repositories;
using System.Threading.Tasks;

namespace BeautySoftBE.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly FirebaseStorageService _firebaseStorageService;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserModel?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(UserModel user, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imageUrl = await _firebaseStorageService.UploadImageAsync(stream, imageFile.FileName);
                user.Avatar = imageUrl;
            }
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }
    }
}