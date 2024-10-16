using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.IdentityModel.Tokens.Jwt;

namespace MyBotAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [RequiredScope("jwt")]
    public class JwtInfoController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> GetJwtInfo()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return ["No JWT token found in Authorization header"];
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            IEnumerable<string> claims = jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}");

            return claims;
        }

    }
}
