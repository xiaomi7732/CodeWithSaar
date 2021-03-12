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
        private object _boxedLogin;

        public UserInfo(string userName, object login)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new System.ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
            }

            Name = userName;
            _boxedLogin = login ?? throw new System.ArgumentNullException(nameof(login));
        }

        public string Name { get; }

        public IEnumerable<string> Roles { get; set; }

        public IEnumerable<Claim> AdditionalClaims { get; private set; } = Enumerable.Empty<Claim>();

        public void AppendAdditionalClaims(params Claim[] claims)
            => AdditionalClaims = AdditionalClaims.Union(claims);

        public bool TryUnboxLogin<T>(out T login)
            where T : class
        {
            login = default;
            if(_boxedLogin is T unboxed)
            {
               login = unboxed;
               return true; 
            }
            return false;
        }
    }
}