using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public class JWTAuthOptions
    {
        public static string SectionName = "JWTAuth";

        public string IssuerSigningSecret { get; set; }
        public string Issuer { get; set; } = "JWTAuthIssuer";
        public string Audience { get; set; } = "JWTAuthAudience";

        public string RoleClaimType { get; set; } = ClaimTypes.Role;
        public string NameClaimType { get; set; } = ClaimTypes.Name;

        public TimeSpan TokenLifeSpan { get; set; } = TimeSpan.FromHours(4);

        public PathString TokenPath { get; set; } = "/token";

        public Func<MessageReceivedContext, Task> OnJWTAuthenticationMessageReceived { get; set; }

        public Func<string, IServiceProvider, Task<UserInfo>> OnValidateUserInfo { get; set; } = (token, p) => Task.FromResult<UserInfo>(null);

        public Func<UserInfo, IServiceProvider, Task<IEnumerable<string>>> OnValidateRoleInfo { get; set; } = (user, p) => Task.FromResult(Enumerable.Empty<string>());
    }
}