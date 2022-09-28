namespace CodeWithSaar.FishCard.Auth;
public interface IUserService
{
    Task<bool> IsValidUserAsync(LoginCredential user, CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetRolesAsync(string userName, CancellationToken cancellationToken);
}