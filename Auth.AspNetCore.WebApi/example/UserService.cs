using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;

namespace QuickStart.WebAPI
{
    internal class UserService : UserServiceBase<DefaultUserLogin>
    {
        // User lookup dictionary. Should be in a database or something, password hashed.
        private Dictionary<string, byte[]> _inMemoryUserDB = new Dictionary<string, byte[]>()
        {
            ["saar"] = GetPasswordHash("123"),
            ["adam"] = GetPasswordHash("123"),
        };


        private HashSet<(string userName, string role)> _userRoleMapping = new HashSet<(string userName, string role)>(){
            ("saar", "User"),
            ("saar", "Admin"),
            ("adam", "User")
        };

        protected override async Task<UserInfo> IsValidUserAsync(DefaultUserLogin login)
        {
            // This is just an example with hard-coded values.
            // Check with database or other service to making sure the user info is valid.
            byte[] passwordHash = await GetHashedPasswordFromDBOrSomewhereElseAsync(login.Username).ConfigureAwait(false);
            if (passwordHash == null)
            {
                // Username doesn't match
                return null;
            }

            byte[] loginPasswordHash = GetPasswordHash(login.Password);
            if (!passwordHash.SequenceEqual(loginPasswordHash))
            {
                // Password doesn't match
                return null;
            }

            // Create UserInfo
            return new UserInfo(login.Username);
        }

        public override async Task ValidateRolesAsync(UserInfo userInfo)
        {
            userInfo.Roles = await GetRolesFromDBOrSomewhereElseAsync(userInfo.Name).ConfigureAwait(false);
        }

        /// <summary>
        /// Simulates getting roles from a database.
        /// </summary>
        /// <param name="userName">The user name for the target user.</param>
        private Task<IEnumerable<string>> GetRolesFromDBOrSomewhereElseAsync(string userName)
        {
            return Task.FromResult(
                _userRoleMapping
                .Where(mapping => string.Equals(mapping.userName, userName, StringComparison.OrdinalIgnoreCase))
                .Select(mapping => mapping.role)
            );
        }

        private Task<byte[]> GetHashedPasswordFromDBOrSomewhereElseAsync(string userName)
        {
            byte[] passwordHash = null;
            // This is just an example with hard-coded values.
            // Check with database or other service to making sure the user info is valid.
            _inMemoryUserDB.TryGetValue(userName, out passwordHash);
            return Task.FromResult(passwordHash);
        }

        private static byte[] GetPasswordHash(string value)
        {
            using SHA256 sha256Hash = SHA256.Create();
            return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value));
        }
    }
}