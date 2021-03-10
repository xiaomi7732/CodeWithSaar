using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace JWTAuth.AspNetCore.WebAPI
{
    /// <summary>
    /// Basic user info.
    /// </summary>
    public class UserInfo
    {
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public IEnumerable<Claim> AdditionalClaims { get; } = Enumerable.Empty<Claim>();

        public void AppendAdditionalClaims(params Claim[] claims)
            => AdditionalClaims.Union(claims);
    }
}