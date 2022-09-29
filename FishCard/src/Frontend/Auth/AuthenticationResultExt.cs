using CodeWithSaar.FishCard.Models.Auth;

namespace CodeWithSaar.FishCard.Auth;

public class AuthenticationResultExt
{
    public AuthenticationResult AuthenticationResult { get; }

    public AuthenticationResultExt(AuthenticationResult authenticationResult)
    {
        AuthenticationResult = authenticationResult ?? throw new ArgumentNullException(nameof(authenticationResult));
    }

    public string UserName { get; set; } = default!;
}