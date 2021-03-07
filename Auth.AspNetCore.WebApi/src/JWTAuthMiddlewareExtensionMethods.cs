using JWTAuth.AspNetCore.WebAPI;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JWTAuthMiddlewareExtensionMethods
    {
        public static IApplicationBuilder UseJWTAuth(this IApplicationBuilder builder)
        {
            return builder
                    .UseMiddleware<TokenMiddleware>()
                    .UseAuthentication()
                    .UseAuthorization();
        }
    }
}