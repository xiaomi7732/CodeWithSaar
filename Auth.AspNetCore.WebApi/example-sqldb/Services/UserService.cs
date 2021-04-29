using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

            byte[] passwordHash = await ComputePasswordHashAsync(password).ConfigureAwait(false);
            User newUser = new User { Name = userName, PasswordHash = passwordHash };
            UserDBContext.Add<User>(newUser);
            await UserDBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return newUser; // New user with id.
        }

        public async IAsyncEnumerable<User> ListUsersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (User user in UserDBContext.Users.AsAsyncEnumerable<User>())
            {
                RedactSensitiveData(user);
                yield return user;
            }
        }

        public Task<User> GetUserByNameAsync(string userName)
        {
            return UserDBContext.Users.SingleAsync(u => string.Equals(userName, u.Name));
        }
        public async Task<User> GetValidUserAsync(string userName, string clearTextPassword, CancellationToken cancellationToken = default)
        {
            User target = await GetUserByNameAsync(userName).ConfigureAwait(false);
            byte[] actualPasswordHash = await ComputePasswordHashAsync(clearTextPassword).ConfigureAwait(false);
            if (actualPasswordHash.SequenceEqual(target.PasswordHash))
            {
                return target;
            }
            return null;
        }

        public Task ChangePasswordAsync(User user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByIdAsync(
            Guid userId,
            Func<IQueryable<User>, IQueryable<User>> onUserFetched = null,
            Func<User, User> onReturnUser = null,
            CancellationToken cancellationToken = default)
        {
            onUserFetched ??= users => users;
            onReturnUser ??= RedactSensitiveData;
            IQueryable<User> users = UserDBContext.Users;
            users = onUserFetched(users);
            
            User target = await users.SingleAsync(u => u.Id == userId);
            return onReturnUser(target);
        }

        public async Task<IEnumerable<Role>> GetRoles(User user)
        {
            User targetUser = await UserDBContext.Users.Include(u => u.Roles).SingleAsync(u => u.Id == user.Id);
            return targetUser.Roles;
        }

        private static User RedactSensitiveData(User user)
        {
            // Redact the password hash
            user.PasswordHash = null;
            return user;
        }

        private async Task<byte[]> ComputePasswordHashAsync(string clearTextPassword)
        {
            using SHA256 sha256 = SHA256.Create();
            using Stream passwordStream = clearTextPassword.ToStream();
            byte[] passwordHash = await sha256.ComputeHashAsync(passwordStream).ConfigureAwait(false);
            return passwordHash;
        }
    }
}