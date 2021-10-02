using System;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;

namespace CodeNameK.FollowSDKDoc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string[] scopes = { "User.Read" };
            string clientId = "c35c09da-4ac5-47e7-98cc-2ba2979d8ae7";
            string tenantId = "common";
            InteractiveBrowserCredentialOptions interactiveBrowserCredentialOptions = new InteractiveBrowserCredentialOptions()
            {
                ClientId = clientId,
                TenantId = tenantId,
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                RedirectUri = new Uri("http://localhost"),
            };
            InteractiveBrowserCredential interactiveBrowserCredential = new InteractiveBrowserCredential(interactiveBrowserCredentialOptions);

            GraphServiceClient graphClient = new GraphServiceClient(interactiveBrowserCredential, scopes); // you can pass the TokenCredential directly to the GraphServiceClient

            User me = await graphClient.Me.Request()
                            .GetAsync();
            Console.WriteLine(me.DisplayName);
        }
    }
}
