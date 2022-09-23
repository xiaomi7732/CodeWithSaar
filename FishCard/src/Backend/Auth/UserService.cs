namespace CodeWithSaar.FishCard.Auth;

public class UserService : IUserService
{
    const string hardcodedUserName = "admin";
    const string hardcodedPassword = "password123!";

    public Task<IEnumerable<string>> GetRolesAsync(string userName, CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<string>() { "Admin" }.AsEnumerable());
    }

    public Task<bool> IsValidUserAsync(User user, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(false);
        }
        return Task.FromResult(string.Equals(user?.UserName, hardcodedUserName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(GetHashedPassword(user?.Password), hardcodedPassword, StringComparison.Ordinal));
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