using System.Net;
using CodeWithSaar.FishCard.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CodeWithSaar.FishCard.Apps;

public class AuthRouteView : RouteView
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IAuthenticationService AuthenticationService { get; set; } = default!;

    protected override void Render(RenderTreeBuilder builder)
    {
        bool authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
        if (authorize && AuthenticationService.AuthenticationResult is null)
        {
            var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
            NavigationManager.NavigateTo($"login?returnUrl={returnUrl}");
        }
        else
        {
            base.Render(builder);
        }
    }
}