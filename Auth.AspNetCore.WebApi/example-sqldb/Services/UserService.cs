using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace JWT.Example.WithSQLDB
{
    public class UserService : UserDBBase
    {
        public UserService(UserDBContext userDatabase)
            : base(userDatabase)
        {
        }

        public async Task<User> AddUserAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new System.ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new System.ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            using SHA256 sha256 = SHA256.Create();
            using Stream passwordStream = password.ToStream();
            byte[] passwordHash = await sha256.ComputeHashAsync(passwordStream).ConfigureAwait(false);
            User newUser = new User { Name = userName, PasswordHash = passwordHash };
            UserDBContext.Add<User>(newUser);
            await UserDBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return newUser; // New user with id.
        }

        public async IAsyncEnumerable<User> ListUsersAsync([EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            await foreach (User user in UserDBContext.Users.AsAsyncEnumerable<User>())
            {
                // Redact the password hash
                user.PasswordHash = null;
                yield return user;
            }
        }

        public Task ChangePasswordAsync(User user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Role>> GetRoles(User user)
        {
            User targetUser = await UserDBContext.Users.Include(u => u.Roles).SingleAsync(u => u.Id == user.Id);
            return targetUser.Roles;
        }
    }
}