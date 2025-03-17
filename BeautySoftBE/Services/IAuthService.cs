using BeautySoftBE.Application.DTOs;

namespace BeautySoftBE.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDTO model);
    Task<string?> LoginAsync(LoginDTO model);
    Task<string?> RefreshToken(string email);
}