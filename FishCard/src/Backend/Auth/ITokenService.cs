namespace CodeWithSaar.FishCard.Auth;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(User validUser, CancellationToken cancellationToken);
}