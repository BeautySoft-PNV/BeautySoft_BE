using System.Security.Claims;

namespace BeautySoftBE.Services;

public interface IJWTService
{
    ClaimsPrincipal? ValidateToken(string token);
}