namespace CodeNameK.Contracts.CustomOptions
{
    public class MSALAppOptions<T>
    {
        public string? ClientId { get; set; }
        public string TenantId { get; set; } = "common";
        public string[]? Scopes { get; set; }
        public string? RedirectUri { get; set; }
    }
}