using CodeWithSaar.FishCard.DataAccess;
using Microsoft.EntityFrameworkCore;
using DbUser = CodeWithSaar.FishCard.Models.User;
namespace CodeWithSaar.FishCard.Auth;

public class UserService : IUserService
{
    private readonly UserManagerContext _userDb;

    public UserService(UserManagerContext userDb)
    {
        _userDb = userDb ?? throw new ArgumentNullException(nameof(userDb));
    }

    public async Task<IEnumerable<string>> GetRolesAsync(string userName, CancellationToken cancellationToken)
    {
        List<DbUser>? userQuery = await _userDb.Users.Include(user => user.Roles).Where(u => u.UserName == userName).ToListAsync(cancellationToken);

        if (userQuery is null || !userQuery.Any())
        {
            return Enumerable.Empty<string>();
        }
        return userQuery[0].Roles.Select(r => r.RoleId);
    }

    public async Task<bool> IsValidUserAsync(User user, CancellationToken cancellationToken)
    {
        List<DbUser> userQuery = await _userDb.Users.Where(u => u.UserName == user.UserName).ToListAsync(cancellationToken);
        if (!userQuery.Any())
        {
            return false;
        }

        string? hashedPassword = GetHashedPassword(user.Password);
        if (string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        if (!string.Equals(hashedPassword, userQuery[0].HashedPassword, StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private string? GetHashedPassword(string? clearTextPassword)
    {
        if (string.IsNullOrEmpty(clearTextPassword))
        {
            return null;
        }
        // TODO: Hash clare text
        return clearTextPassword;
    }
}