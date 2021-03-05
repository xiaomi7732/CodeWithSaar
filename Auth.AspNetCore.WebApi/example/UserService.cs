using System;
using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;

namespace QuickStart.WebAPI
{
    internal class UserService : UserServiceBase<Login>
    {
        protected override Task<UserInfo> IsValidUserAsync(Login login)
        {
            // This is just an example with hard-coded values.
            // Check with database or other service to making sure the user info is valid.
            if (string.Equals(login.UserName, "saars", StringComparison.OrdinalIgnoreCase)
                && string.Equals(login.Password, "123", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new UserInfo()
                {
                    Name = "saars",
                    // Also query the database or other service to get the proper role info.
                    // This is optional if you don't want to support role based access control.
                    Roles = new[] { "User", "Admin" },
                });
            }
            return Task.FromResult((UserInfo)default);
        }
    }
}