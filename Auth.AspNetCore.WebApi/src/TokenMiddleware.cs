using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuth.AspNetCore.WebAPI
{
    /// <summary>
    /// Middle ware to intercept token request.
    /// </summary>
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            this._next = next ?? throw new System.ArgumentNullException(nameof(next));
        }

        public async Task Invoke(
            HttpContext httpContext,
            IUserValidationService userValidationService,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<JWTAuthOptions> options)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (userValidationService is null)
            {
                throw new InvalidOperationException("No IUserValidationService registered.");
            }

            if (serviceScopeFactory is null)
            {
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            }

            JWTAuthOptions jwtAuthOptions = options?.Value ?? new JWTAuthOptions();
            PathString requestPath = httpContext.Request.Path;
            if (!IsTokenPath(httpContext, jwtAuthOptions.TokenPath))
            {
                await _next.Invoke(httpContext);
                return;
            }

            using StreamReader sr = new StreamReader(httpContext.Request.Body);
            string jsonContent = await sr.ReadToEndAsync().ConfigureAwait(false);

            try
            {
                UserInfo validUser = await userValidationService.ValidateUserAsync(jsonContent).ConfigureAwait(false);
                if (validUser is null)
                {
                    WriteUnauthorized(httpContext);
                    return; // Do NOT pass user validation.
                }

                // Fetching the role info.
                validUser.Roles = await ValidateRoleInfo(validUser, serviceScopeFactory).ConfigureAwait(false);

                string accessToken = BuildAccessToken(jwtAuthOptions, validUser);
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                string responseBody = JsonSerializer.Serialize(
                    new TokenResponseBody() { Token = accessToken },
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    });
                await httpContext.Response.WriteAsync(responseBody).ConfigureAwait(false);
            }
            catch (InvalidCastException)
            {
                await httpContext.Response.WriteAsync("Parsing Login info failed. Is it in valid json format?").ConfigureAwait(false);
                return;
            }
        }

        private void WriteUnauthorized(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private Task<IEnumerable<string>> ValidateRoleInfo(UserInfo validUser, IServiceScopeFactory serviceScopeFactory)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            IRoleValidationService roleValidationService = scope.ServiceProvider.GetService<IRoleValidationService>();
            if (roleValidationService == null)
            {
                // No role validation service exist, treat role validation as a success.
                return Task.FromResult(Enumerable.Empty<string>());
            }
            return roleValidationService.ValidateRolesAsync(validUser);
        }

        private string BuildAccessToken(JWTAuthOptions jwtAuthOptions, UserInfo userInfo)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(jwtAuthOptions.IssuerSigningSecret);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: jwtAuthOptions.Issuer,
                audience: jwtAuthOptions.Audience,
                claims: GetClaims(userInfo, jwtAuthOptions),
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(jwtAuthOptions.TokenLifeSpan),
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );

            return tokenHandler.WriteToken(jwtSecurityToken);
        }

        private IEnumerable<Claim> GetClaims(UserInfo user, JWTAuthOptions options)
        {
            yield return new Claim(options.NameClaimType, user.Name);
            foreach (string role in user.Roles.NullAsEmpty())
            {
                yield return new Claim(options.RoleClaimType, role);
            }
            if (user.AdditionalClaims.NullAsEmpty().Any())
            {
                foreach (var additionalClaim in user.AdditionalClaims)
                {
                    yield return additionalClaim;
                }
            }
        }

        private bool IsTokenPath(HttpContext httpContext, PathString configuredTokenPath)
            => httpContext.Request.Method == HttpMethods.Post && httpContext.Request.Path.HasValue && httpContext.Request.Path == configuredTokenPath;
    }
}