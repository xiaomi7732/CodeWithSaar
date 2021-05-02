using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Example.WithSQLDB
{
    public class TokenService
    {
        private readonly JWTAuthOptions _options;
        private readonly UserService _userService;

        public TokenService(
            IOptions<JWTAuthOptions> options,
            UserService userService)
        {
            this._options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this._userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
        }

        public async Task<string> GetAccessTokenAsync(string userName, string clearTextPassword, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new System.ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
            }

            if (string.IsNullOrEmpty(clearTextPassword))
            {
                throw new System.ArgumentException($"'{nameof(clearTextPassword)}' cannot be null or empty.", nameof(clearTextPassword));
            }
            User validUser = await  _userService.GetValidUserAsync(userName, clearTextPassword, cancellationToken).ConfigureAwait(false);
            if(validUser == null)
            {
                return null;
            }

            IEnumerable<Role> roles = await _userService.GetRoles(validUser).ConfigureAwait(false);

            // Package: System.IdentityModel.Tokens.Jwt
            List<Claim> claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, userName),
            };
            foreach(Role role in roles)
            {
                claims.Add(new Claim("Role", role.Name));
            }

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "saar",
                audience: "saar-audience",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: new SigningCredentials(
                    key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
                    algorithm: SecurityAlgorithms.HmacSha256
                )
             );

            return (new JwtSecurityTokenHandler()).WriteToken(token);
        }
    }
}