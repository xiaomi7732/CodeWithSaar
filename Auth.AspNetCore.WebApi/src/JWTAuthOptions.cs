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
        public const string SectionName = "JWTAuth";

        public string IssuerSigningSecret { get; set; }
        public string Issuer { get; set; } = "JWTAuthIssuer";
        public string Audience { get; set; } = "JWTAuthAudience";

        public string RoleClaimType { get; set; } = ClaimTypes.Role;
        public string NameClaimType { get; set; } = ClaimTypes.Name;

        public TimeSpan TokenLifeSpan { get; set; } = TimeSpan.FromHours(4);

        public PathString TokenPath { get; set; } = "/token";

        /// <summary>
        /// A delegate that will be invoked when JWT authentication message is received. This is an extension point to allow manipulating the authentication context.
        /// </summary>
        public Func<MessageReceivedContext, Task> OnJWTAuthenticationMessageReceived { get; set; }

        /// <summary>
        /// Gets or sets a delegate when user validation is needed. The delegate takes in a string as input, and returns a valid user info.
        /// The string input will contain the POST request body for TokenPath.
        /// </summary>
        /// <returns>Returns a task that will have valid user information.</returns>
        public Func<string, IServiceProvider, Task<UserInfo>> OnValidateUserInfo { get; set; } = (token, p) => Task.FromResult<UserInfo>(null);

        /// <summary>
        /// Gets or sets a delegate when role validation is needed. The delegate takes in a valid user, it is expected to fetch the roles the user is in
        /// as a list of string and returned as an async task.
        /// </summary>
        /// <returns>Returns a task, upon completion, gets the list of roles the user is in.</returns>
        public Func<UserInfo, IServiceProvider, Task<IEnumerable<string>>> OnValidateRoleInfo { get; set; } = (user, p) => Task.FromResult(Enumerable.Empty<string>());
    }
}