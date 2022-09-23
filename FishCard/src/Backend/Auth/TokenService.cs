using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CodeWithSaar.FishCard.Auth;

internal class TokenService : ITokenService
{
    private readonly AuthOptions _options;
    private readonly IUserService _userService;

    public TokenService(
        IOptions<AuthOptions> options,
        IUserService userService)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<string> GetAccessTokenAsync(User validUser, CancellationToken cancellationToken)
    {
        List<Claim> claims = new List<Claim>{
            new Claim(ClaimTypes.Name, validUser.UserName),
        };
        foreach (string role in await _userService.GetRolesAsync(validUser.UserName, cancellationToken))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_options.ExpiryTimeSpan),
            signingCredentials: new SigningCredentials(
                key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.TokenSigningKey)),
                algorithm: SecurityAlgorithms.HmacSha256
            )
         );

        return (new JwtSecurityTokenHandler()).WriteToken(token);
    }
}