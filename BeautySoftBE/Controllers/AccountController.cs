using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BeautySoftBE.Application.DTOs;
using BeautySoftBE.Data;
using BeautySoftBE.Models;
using BeautySoftBE.Repositories;
using BeautySoftBE.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BeautySoftBE.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : Controller
{
    private readonly EmailService _emailSender;
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly JWTService _jwtService;
    private readonly IConfiguration _configuration;

    public AccountController( EmailService emailSender, ApplicationDbContext context,  IUserRepository userRepository, JWTService jwtService, IConfiguration configuration)
    {
        _emailSender = emailSender;
        _context = context;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _configuration = configuration;
    }
    [HttpGet("reset-password")]
    public IActionResult ResetPassword(string token, string email)
    {
        var model = new ResetPasswordRequest { Token = token, Email = email };
        return View(model);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
            return NotFound(new { message = "Email does not exist!" });

        var token = GenerateJwtToken(user);
        var resetLink = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);

        await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Click on <a href='{resetLink}'>đây</a> to reset password.");

        return Ok(new { message = "Password reset email has been sent!" });
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest model)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        var principal = _jwtService.ValidateToken(token);
        if (principal == null)
        {
            return Unauthorized(new { message = "Token is invalid or expired!" });
        }
        
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid data!", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        var user = await _userRepository.GetEmailByUsernameAsync(model.Email);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid email!" });
        }

        if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
        {
            return BadRequest(new { message = "Password must be at least 6 characters!" });
        }

        user.Password = HashPassword(model.NewPassword);
        await _userRepository.UpdateAsync(user);

        return Ok(new { message = "Password reset successfully!" });
    }

    
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
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
            Expires = DateTime.UtcNow.AddSeconds(60),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}