using System;
using System.Security.Claims;
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
    }
}