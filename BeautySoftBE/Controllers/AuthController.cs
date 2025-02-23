using BeautySoftBE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BeautySoftBE.Application.DTOs;
using BeautySoftBE.Services;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var result = await _authService.RegisterAsync(model);
        if (result == "Tài khoản đã tồn tại!")
            return BadRequest(new { message = result });

        return Ok(new { message = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var token = await _authService.LoginAsync(model);
        if (token == null)
            return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu!" });

        return Ok(new { token });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Đăng xuất thành công!" });
    }
}