using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JWT.Example.WithSQLDB
{
    public class UserService
    {
        private readonly UserDBContext _userDatabase;

        public UserService(UserDBContext userDatabase)
        {
            _userDatabase = userDatabase ?? throw new System.ArgumentNullException(nameof(userDatabase));
        }

        public async Task CreateAsync(string userName, string password, CancellationToken cancellationToken = default)
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

            _userDatabase.Add<User>(new User { Name = userName, PasswordHash = passwordHash });
            await _userDatabase.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            if (roleName is null)
            {
                throw new System.ArgumentNullException(nameof(roleName));
            }
            await _userDatabase.Roles.AddAsync(new Role() { Name = roleName }, cancellationToken).ConfigureAwait(false);
            await _userDatabase.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task AddRoleAssignmentAsync(string userName, string roleName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException($"'{nameof(roleName)}' cannot be null or empty.", nameof(roleName));
            }

            User targetUser = null;
            try
            {

                targetUser = GetFirstUserByName(userName);
                if (targetUser is null)
                {
                    throw new InvalidOperationException($"Target user {userName} is not found.");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                throw;
            }

            Role targetRole = GetFirstRoleByName(roleName);
            if (targetRole is null)
            {
                throw new InvalidOperationException($"Target role {roleName} is not found.");
            }

            if (targetUser.Roles.FirstOrDefault(r => string.Equals(r.Name, targetRole.Name)) is null)
            {
                targetUser.Roles.Add(targetRole);
            }
            await _userDatabase.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private User GetFirstUserByName(string userName)
            => _userDatabase.Users.Include(u => u.Roles).SingleOrDefault(u => string.Equals(u.Name, userName));

        private Role GetFirstRoleByName(string roleName)
            => _userDatabase.Roles.SingleOrDefault(r => string.Equals(r.Name, roleName));
    }
}