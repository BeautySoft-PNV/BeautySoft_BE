using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
public class BaseController : ControllerBase
{
    protected int? GetUserIdFromToken()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            var userIdClaim = identity.FindFirst("id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
        }
        return null;
    }
}