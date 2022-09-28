namespace CodeWithSaar.FishCard.Auth;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(LoginCredential validUser, CancellationToken cancellationToken);
}