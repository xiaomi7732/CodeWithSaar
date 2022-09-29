namespace CodeWithSaar.FishCard.Auth;

public interface IAuthenticationService
{
    AuthenticationResultExt? AuthenticationResult { get; }
    Task ResumeLoggingStateAsync(CancellationToken cancellationToken);

    Task LoginAsync(LoginCredential credential, string? redirectUrl, CancellationToken cancellationToken);

    Task LogoutAsync(CancellationToken cancellationToken);
}