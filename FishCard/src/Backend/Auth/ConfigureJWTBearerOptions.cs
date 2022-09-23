using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CodeWithSaar.FishCard.Auth;

public class ConfigureJWTBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthOptions _options;
    private readonly ILogger _logger;

    public ConfigureJWTBearerOptions(IOptions<AuthOptions> options, ILogger<ConfigureJWTBearerOptions> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public void Configure(JwtBearerOptions options)
    {
        _logger.LogInformation("No name Configure called of {className}", nameof(ConfigureJWTBearerOptions));
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        _logger.LogInformation("{name}.{methodName} is called. Policy name: {policyName}", nameof(ConfigureJWTBearerOptions), nameof(Configure), name);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.TokenSigningKey)),
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
        };
    }
}
