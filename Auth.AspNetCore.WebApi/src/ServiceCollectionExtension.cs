using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using JWTAuth.AspNetCore.WebAPI;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to help registering JWT auth related services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add JWT authentication related services.
        /// </summary>
        public static IServiceCollection AddJWTAuth(this IServiceCollection services, Action<JWTAuthOptions> configure = null)
        {
            IServiceProvider provider = services.BuildServiceProvider();
            IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
            ILogger logger = provider.GetService<ILogger<TokenMiddleware>>();

            JWTAuthOptions jwtAuthOptions = configuration.GetSection(JWTAuthOptions.SectionName).Get<JWTAuthOptions>() ?? new JWTAuthOptions();
            configure?.Invoke(jwtAuthOptions);
            if (string.IsNullOrEmpty(jwtAuthOptions.IssuerSigningSecret))
            {
                logger?.LogWarning("Issuer signing secret is not specified. Using random string as secrets temperory. Please specify issuer signing secret.");
                jwtAuthOptions.IssuerSigningSecret = Guid.NewGuid().ToString();
            }

            IOptions<JWTAuthOptions> newOptionInstance = Options.Options.Create(jwtAuthOptions);
            ServiceDescriptor replaceOption = new ServiceDescriptor(typeof(IOptions<JWTAuthOptions>), newOptionInstance);
            services.Replace(replaceOption);

            services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.IssuerSigningSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtAuthOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAuthOptions.Audience,
                    NameClaimType = jwtAuthOptions.NameClaimType,
                    RoleClaimType = jwtAuthOptions.RoleClaimType,
                };
            });

            return services;
        }
    }
}