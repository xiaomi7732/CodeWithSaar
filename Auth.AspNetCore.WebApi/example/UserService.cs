using System;
using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;

namespace QuickStart.WebAPI
{
    internal class UserService : UserServiceBase<Login>
    {
        protected override Task<UserInfo> IsValidUserAsync(Login login)
        {
            if (string.Equals(login.UserName, "saars", StringComparison.OrdinalIgnoreCase)
                && string.Equals(login.Password, "123", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new UserInfo()
                {
                    Name = "saars",
                    Roles = new[] { "User", "Admin" }
                });
            }
            return Task.FromResult((UserInfo)default);
        }
    }
}