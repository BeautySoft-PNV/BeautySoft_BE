using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
public class BaseController : ControllerBase
{
    protected int? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
            
        if (!handler.CanReadToken(token))
        {
            Console.WriteLine("Token is invalid or corrupted.");
            return null;
        }

        var jsonToken = handler.ReadJwtToken(token);

        var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
    
        return userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;
    }
    
    protected string? GetUserEmailFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
            
        if (!handler.CanReadToken(token))
        {
            Console.WriteLine("Token is invalid or corrupted.");
            return null;
        }

        var jsonToken = handler.ReadJwtToken(token);

        var userEmailClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
    
        return userEmailClaim;
    }

}