using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace BeautySoftBE.Middleware // Đặt theo namespace của project bạn
{
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult(); // 401 Unauthorized
                return;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                if (role != "ADMIN")
                {
                    context.Result = new ForbidResult(); // 403 Forbidden
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult(); // 401 Unauthorized
            }
        }
    }
}