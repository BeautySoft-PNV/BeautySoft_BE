using BeautySoftBE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BeautySoftBE.Application.DTOs;
using BeautySoftBE.Services;

[Route("api/auth")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    public static HashSet<string> BlacklistedTokens = new HashSet<string>();
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var result = await _authService.RegisterAsync(model);
        if (result == "Account already exists!")
            return BadRequest(new { message = result });

        return Ok(new { message = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var token = await _authService.LoginAsync(model);
        if (token == null)
            return Unauthorized(new { message = "Wrong account or password!" });
        if (token == "BLOCKED")
        {
            return StatusCode(403, "Your account has been locked.");
        }
        return Ok(new { token });
    }
    
        [HttpPost("refreshToken")]
        public async Task<IActionResult> refreshToken()
        {
            var oldToken = HttpContext.Request.Headers["Authorization"].ToString();
                
            if (oldToken.StartsWith("Bearer "))
            {
                oldToken = oldToken.Substring(7).Trim();
            }

            var userEmail = GetUserEmailFromToken(oldToken);
            
            var token = await _authService.RefreshToken(userEmail);
            
            return Ok(new { token });
        }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "Token is required!" });
        }

        if (token.StartsWith("Bearer "))
        {
            token = token.Substring(7).Trim();
        }
        
        BlacklistedTokens.Add(token);

        return Ok(new { message = "Logged out successfully!" });
    }
    
    public bool IsTokenValid(string token)
    {
        return !BlacklistedTokens.Contains(token);
    }

}