using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using CodeWithSaar.FishCard.Apps;
using CodeWithSaar.FishCard.Models.Auth;
using Microsoft.AspNetCore.Components;

namespace CodeWithSaar.FishCard.Auth;

internal class AuthenticationService : IAuthenticationService
{
    private const string AccessTokenKey = "accessToken";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public AuthenticationService(
        IHttpClientFactory httpClientFactory,
        ILocalStorageService localStorageServiceFactory,
        NavigationManager navigationManager,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _localStorageService = localStorageServiceFactory ?? throw new ArgumentNullException(nameof(localStorageServiceFactory));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _jsonSerializerOptions = jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
    }

    public AuthenticationResultExt? AuthenticationResult { get; private set; }

    public async Task LoginAsync(LoginCredential credential, string? redirectUrl, CancellationToken cancellationToken)
    {
        using (HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientName.Backend))
        {
            HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync<LoginCredential>(
                new Uri("/token", UriKind.Relative),
                credential);
            if (responseMessage.IsSuccessStatusCode)
            {
                AuthenticationResult? authResult = await responseMessage.Content.ReadFromJsonAsync<AuthenticationResult>(options: _jsonSerializerOptions, cancellationToken);
                if (authResult is not null)
                {
                    AuthenticationResult = new AuthenticationResultExt(authResult) { UserName = credential.UserName };
                    await _localStorageService.SetItemAsync(GetKey(AccessTokenKey), AuthenticationResult);

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        _navigationManager.NavigateTo(redirectUrl, false);
                    }
                }
            }
        }
    }

    public Task LogoutAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task ResumeLoggingStateAsync(CancellationToken cancellationToken)
    {

        AuthenticationResult = await _localStorageService.GetItemAsync<AuthenticationResultExt>(GetKey(AccessTokenKey), cancellationToken);
    }

    private string GetKey(string id)
    {
        return $"{this.GetType().FullName}.{id}";
    }
}