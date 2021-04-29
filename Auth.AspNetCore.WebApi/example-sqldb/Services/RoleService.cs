using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JWT.Example.WithSQLDB
{
    public class RoleService : UserDBBase
    {
        public RoleService(UserDBContext userDBContext) : base(userDBContext)
        {

        }

        public IAsyncEnumerable<Role> ListRoles() => UserDBContext.Roles.AsAsyncEnumerable();

        public async Task<Role> AddRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            if (roleName is null)
            {
                throw new System.ArgumentNullException(nameof(roleName));
            }
            Role newRole = new Role() { Name = roleName };
            await UserDBContext.Roles.AddAsync(newRole, cancellationToken).ConfigureAwait(false);
            await UserDBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return newRole;
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
            await UserDBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task DisableRoleAsync(Role role)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetUsers(Role role)
        {
            Role targetRole = await UserDBContext.Roles.Include(r => r.Users).SingleAsync(r => r.Id == role.Id);
            return targetRole.Users;
        }

        private User GetFirstUserByName(string userName)
            => UserDBContext.Users.Include(u => u.Roles).SingleOrDefault(u => string.Equals(u.Name, userName));

        private Role GetFirstRoleByName(string roleName)
            => UserDBContext.Roles.SingleOrDefault(r => string.Equals(r.Name, roleName));
    }
}