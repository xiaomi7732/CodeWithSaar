using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace CodeNameK.InteractiveFlow
{
    class Program
    {
        private static IPublicClientApplication application;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Interactive flow");

            string[] scopes = new[] { "Files.ReadWrite.All" };
            // string[] scopes = new[] { "User.Read" };
            string clientId = "45d8bdbc-bd15-4f96-868d-3faf1aa7d34a";
            string tenantId = "common";
            // string tenantId = "common";

            InteractiveBrowserCredentialOptions options = new InteractiveBrowserCredentialOptions
            {
                TenantId = tenantId,
                ClientId = clientId,
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                // RedirectUri = new Uri("https://login.microsoftonline.com/common/oauth2/nativeclient"),
                RedirectUri = new Uri("http://localhost"),
            };

            // IPublicClientApplication publicClientApp = PublicClientApplicationBuilder.Create(clientId)
            //     .WithDefaultRedirectUri()
            //     .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
            //     .Build();

            // DeviceCodeCredential dcredential = new DeviceCodeCredential(new DeviceCodeCredentialOptions
            // {
            //     TenantId = tenantId,
            //     ClientId = clientId,
            //     AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            // });

            InteractiveBrowserCredential interactiveCredential = new InteractiveBrowserCredential(options);

            GraphServiceClient graphClient = new GraphServiceClient(interactiveCredential, scopes);
            User me = await graphClient.Me.Request().GetAsync();

            IDriveItemChildrenCollectionPage driveItemPage = await graphClient.Me.Drive.Root.Children.Request().GetAsync().ConfigureAwait(false);

            while (driveItemPage != null)
            {
                foreach (var item in driveItemPage)
                {
                    Console.WriteLine("Item Name:{0}", item.Name);
                }

                if (driveItemPage.NextPageRequest != null)
                {
                    driveItemPage = await driveItemPage.NextPageRequest.GetAsync().ConfigureAwait(false);
                }
                else
                {
                    driveItemPage = null;
                }
            }


            Console.WriteLine("Me: {0}", me.DisplayName);
            // Console.WriteLine("Drive Name: {0}", driveRoot.);
        }

        // private static async Task<string> SignInUserAndGetTokenUsingMSAL(PublicClientApplicationOptions configuration, string[] scopes)
        // {
        //     string authority = string.Concat(configuration.Instance, configuration.TenantId);

        //     // Initialize the MSAL library by building a public client application
        //     IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder.Create(configuration.ClientId)
        //                                             .WithAuthority(authority)
        //                                             .WithDefaultRedirectUri()
        //                                             .Build();


        //     AuthenticationResult result;
        //     try
        //     {
        //         var accounts = await application.GetAccountsAsync();
        //         result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
        //          .ExecuteAsync();
        //     }
        //     catch (MsalUiRequiredException ex)
        //     {
        //         result = await application.AcquireTokenInteractive(scopes)
        //          .WithClaims(ex.Claims)
        //          .ExecuteAsync();
        //     }

        //     return result.AccessToken;
        // }

        // /// <summary>
        // /// Sign in user using MSAL and obtain a token for MS Graph
        // /// </summary>
        // /// <returns></returns>
        // private async static Task<GraphServiceClient> SignInAndInitializeGraphServiceClient(PublicClientApplicationOptions configuration, string[] scopes)
        // {
        //     GraphServiceClient graphClient = new GraphServiceClient(MSGraphURL,
        //         new DelegateAuthenticationProvider(async (requestMessage) =>
        //         {
        //             requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", await SignInUserAndGetTokenUsingMSAL(configuration, scopes));
        //         }));

        //     return await Task.FromResult(graphClient);
        // }
    }
}
