using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Example.WithSQLDB
{
    [ApiController]
    public class TokenController : ControllerBase
    {
        // 1. Not best practice to hardcode any secrets in the code.
        // 2. DRY violation: duplicate to the value in Startup.cs
        private const string Secret = "this is my custom Secret key for authentication";
        private readonly TokenService _tokenService;

        public TokenController(TokenService tokenService)
        {
            this._tokenService = tokenService ?? 
                throw new ArgumentNullException(nameof(tokenService));
        }

        // Returns an JWT token when the login info is valid.
        [AllowAnonymous]
        [Route("token")]
        [HttpPost()]
        public async Task<ActionResult> GetToken([FromBody] UserLoginModel login)
        {
            string accessToken = await _tokenService.GetAccessTokenAsync(login.UserName, login.Password);

            if (string.IsNullOrEmpty(accessToken))
            {
                return Forbid();
            }

            return Ok(new
            {
                token = accessToken,
            });
        }
    }
}