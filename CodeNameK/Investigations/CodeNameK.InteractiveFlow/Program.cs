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
            string[] scopes = new[] { "Files.ReadWrite.All" };
            string clientId = "c35c09da-4ac5-47e7-98cc-2ba2979d8ae7";
            string tenantId = "common";

            InteractiveBrowserCredentialOptions options = new InteractiveBrowserCredentialOptions
            {
                TenantId = tenantId,
                ClientId = clientId,
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                RedirectUri = new Uri("http://localhost"),
            };
            InteractiveBrowserCredential credential = new InteractiveBrowserCredential(options);
            GraphServiceClient graphClient = new GraphServiceClient(credential, scopes);
            Console.WriteLine("Interactive flow");
            await UseGraphClient(graphClient).ConfigureAwait(false);

            DeviceCodeCredential credential2 = new DeviceCodeCredential(new DeviceCodeCredentialOptions
            {
                TenantId = tenantId,
                ClientId = clientId,
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            });
            Console.WriteLine("DeviceCode flow");
            await UseGraphClient(graphClient).ConfigureAwait(false);
        }

        private static async Task UseGraphClient(GraphServiceClient graphClient)
        {
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
        }
    }
}
