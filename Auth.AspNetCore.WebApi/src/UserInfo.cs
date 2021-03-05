using System.Collections.Generic;

namespace JWTAuth.AspNetCore.WebAPI
{
    public class UserInfo
    {
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}