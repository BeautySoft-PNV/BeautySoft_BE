using BeautySoftBE.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BeautySoftBE.Application.DTOs;
using BeautySoftBE.Repositories;
using BeautySoftBE.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<string> RegisterAsync(RegisterDTO model)
    {
        if (await _userRepository.UserExistsAsync(model.Username))
        {
            return"Account already exists!";
        }
        var user = new UserModel
        {
            Name = model.Username,
            Email = model.Email,
            Password = HashPassword(model.Password),
            RoleId = model.RoleId
        };

        await _userRepository.AddUserAsync(user);
        return"Registration successful!";
    }

    public async Task<string?> LoginAsync(LoginDTO model)
    {
        var user = await _userRepository.GetEmailByUsernameAsync(model.Email);
        if (user == null || !VerifyPassword(model.Password, user.Password))
        {
            return null;
        }
        if (user.IsBlocked)
        {
            return "BLOCKED";
        }
        return GenerateJwtToken(user);
    }
    
    public async Task<string?> RefreshToken(string email)
    {
        var user = await _userRepository.GetEmailByUsernameAsync(email);
        return GenerateJwtToken(user);
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        return HashPassword(password) == passwordHash;
    }

    private string GenerateJwtToken(UserModel user)
    {
        var role = _userRepository.GetRoleNameById(user.RoleId) ?? "User";
        var secretKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
        {
            throw new Exception("JWT Key is invalid or too short.");
        }
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? "Unknown"), 
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
}
