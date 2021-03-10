using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;

namespace QuickStart.WebAPI
{
    internal class UserService : UserServiceBase<DefaultUserLogin>
    {
        // User lookup dictionary. Should be in a database or something, password hashed.
        private Dictionary<string, string> _inMemoryUserDB = new Dictionary<string, string>()
        {
            ["saar"] = "123",
            ["adam"] = "123"
        };


        private HashSet<(string userName, string role)> _userRoleMapping = new HashSet<(string userName, string role)>(){
            ("saar", "User"),
            ("saar", "Admin"),
            ("adam", "User")
        };

        protected override Task<UserInfo> IsValidUserAsync(DefaultUserLogin login)
        {
            // This is just an example with hard-coded values.
            // Check with database or other service to making sure the user info is valid.
            if (!_inMemoryUserDB.TryGetValue(login.Username, out string passwordInDatabase))
            {
                // No username
                return null;
            }

            if (!string.Equals(login.Password, passwordInDatabase, StringComparison.Ordinal))
            {
                // Password doesn't match
                return null;
            }

            // Create UserInfo
            return Task.FromResult(new UserInfo()
            {
                Name = login.Username,
            });
        }

        public override Task ValidateRolesAsync(UserInfo userInfo)
        {
            // Query the database or other service to get the proper role info.
            // This is optional if you don't want to support role based access control, return Task.CompletedTask in that case.
            userInfo.Roles = _userRoleMapping
                .Where(mapping => string.Equals(mapping.userName, userInfo.Name, StringComparison.OrdinalIgnoreCase))
                .Select(mapping => mapping.role);
            return Task.CompletedTask;
        }
    }
}