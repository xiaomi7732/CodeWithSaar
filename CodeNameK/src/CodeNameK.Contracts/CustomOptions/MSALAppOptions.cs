using System;

namespace CodeNameK.Contracts.CustomOptions
{
    public class MSALAppOptions<T>
    {
        public string? ClientId { get; set; }
        public string TenantId { get; set; } = "common";
        public string[]? Scopes { get; set; }
        public string? RedirectUri { get; set; }

        public TimeSpan SignInTimeout { get; set; } = TimeSpan.FromMinutes(2);
    }
}