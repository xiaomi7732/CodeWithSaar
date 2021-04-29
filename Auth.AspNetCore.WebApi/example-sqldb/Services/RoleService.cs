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
        private readonly UserService _userService;

        public RoleService(
            UserDBContext userDBContext,
            UserService userService) : base(userDBContext)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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

        public async Task AddRoleAssignmentAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            User targetUser = null;
            targetUser = await _userService.GetUserByIdAsync(
                userId,
                users => users.Include(u => u.Roles),
                onReturnUser: user => user).ConfigureAwait(false);

            if (targetUser is null)
            {
                throw new InvalidOperationException($"Target user {userId} is not found.");
            }

            Role targetRole = await GetRoleByIdAsync(roleId).ConfigureAwait(false);
            if (targetRole is null)
            {
                throw new InvalidOperationException($"Target role {roleId} is not found.");
            }

            if (targetUser.Roles.FirstOrDefault(r => string.Equals(r.Name, targetRole.Name)) is null)
            {
                targetUser.Roles.Add(targetRole);
            }
            await UserDBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<Role> GetRoleByIdAsync(Guid roleId)
            => UserDBContext.Roles.SingleOrDefaultAsync(r => r.Id == roleId);

        public Task DisableRoleAsync(Role role)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetUsers(Role role)
        {
            Role targetRole = await UserDBContext.Roles.Include(r => r.Users).SingleAsync(r => r.Id == role.Id);
            return targetRole.Users;
        }
    }
}