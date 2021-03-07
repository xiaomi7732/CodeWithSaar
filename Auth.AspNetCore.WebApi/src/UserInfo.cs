using System.Collections.Generic;

namespace JWTAuth.AspNetCore.WebAPI
{
    /// <summary>
    /// Basic user info.
    /// </summary>
    public class UserInfo
    {
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}