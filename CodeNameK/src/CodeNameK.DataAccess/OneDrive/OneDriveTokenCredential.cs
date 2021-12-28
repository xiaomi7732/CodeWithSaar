using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Azure.Core;
using Azure.Identity;
using CodeNameK.Contracts;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeNameK.DAL.OneDrive
{
    public class OneDriveTokenCredential : TokenCredential, ITokenCredentialManager<OneDriveCredentialStatus>
    {
        private readonly MSALAppOptions<OneDriveSync> _graphAPIOptions;
        private readonly ILogger _logger;
        private readonly InteractiveBrowserCredentialOptions _credentialTokenOptions;
        private Task _expiryTask = Task.CompletedTask;
        private CancellationTokenSource? _expiryTaskCancellationTokenSource = null;
        private AccessToken _knownAccessToken;
        private InteractiveBrowserCredential? _credential;
        private SemaphoreSlim _signInLock = new SemaphoreSlim(1, 1);

        public OneDriveTokenCredential(
            IOptions<MSALAppOptions<OneDriveSync>> graphAPIOptions,
            ILogger<OneDriveTokenCredential> logger)
        {
            _graphAPIOptions = graphAPIOptions.Value ?? throw new ArgumentNullException(nameof(graphAPIOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _credentialTokenOptions = CreateInteractiveBrowserCredentialOptions();

        }

        public OneDriveCredentialStatus CurrentStatus
        {
            get;
            private set;
        } = OneDriveCredentialStatus.Initial;

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{method} is called.", nameof(GetToken));
            if (!Enumerable.SequenceEqual(requestContext.Scopes, _graphAPIOptions.Scopes))
            {
                _logger.LogError("Unexpected scope.");
                return default;
            }

            if (CurrentStatus != OneDriveCredentialStatus.SignedIn)
            {
                _logger.LogError("Unexpected status: {status}", CurrentStatus);
                return default;
            }

            return _knownAccessToken;
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{method} is called.", nameof(GetTokenAsync));
            return new ValueTask<AccessToken>(GetToken(requestContext, cancellationToken));
        }

        public async Task<OneDriveCredentialStatus> SignInAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            await _signInLock.WaitAsync();
            try
            {
                if (CurrentStatus == OneDriveCredentialStatus.SignedIn)
                {
                    return OneDriveCredentialStatus.SignedIn;
                }

                CurrentStatus = OneDriveCredentialStatus.SigningIn;
                try
                {
                    _credential = _credential ?? new InteractiveBrowserCredential(_credentialTokenOptions);
                    TokenRequestContext context = new TokenRequestContext(_graphAPIOptions.Scopes!);

                    using (CancellationTokenSource timeoutTokenSource = new CancellationTokenSource(timeout))
                    using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token))
                    {
                        _knownAccessToken = await _credential.GetTokenAsync(context, linkedTokenSource.Token);

                        // Setup expiry
                        _expiryTaskCancellationTokenSource?.Cancel();
                        _expiryTaskCancellationTokenSource?.Dispose();
                        _expiryTaskCancellationTokenSource = new CancellationTokenSource();
                        _expiryTask = Task.Run(async () =>
                        {
                            TimeSpan timeToExpiry = (_knownAccessToken.ExpiresOn.UtcDateTime - DateTime.UtcNow).Add(TimeSpan.FromSeconds(-5));
                            _logger.LogDebug("Token expires in: {span}", timeToExpiry);
                            await Task.Delay(timeToExpiry).ConfigureAwait(false);
                            if (CurrentStatus == OneDriveCredentialStatus.SignedIn)
                            {
                                _logger.LogDebug("Token has expired.");
                                CurrentStatus = OneDriveCredentialStatus.Expired;
                            }
                        }, _expiryTaskCancellationTokenSource.Token);

                        CurrentStatus = OneDriveCredentialStatus.SignedIn;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex, "Sign in timed out.");
                    CurrentStatus = OneDriveCredentialStatus.Failed;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error signing in.");
                    CurrentStatus = OneDriveCredentialStatus.Failed;
                }

                return CurrentStatus;
            }
            finally
            {
                _signInLock.Release();
            }
        }

        private InteractiveBrowserCredentialOptions CreateInteractiveBrowserCredentialOptions()
        {
            return new InteractiveBrowserCredentialOptions
            {
                TenantId = _graphAPIOptions.TenantId,
                ClientId = _graphAPIOptions.ClientId,
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                RedirectUri = new Uri(_graphAPIOptions.RedirectUri),
            };
        }
    }
}