namespace JWTAuth.AspNetCore.WebAPI
{
    /// <summary>
    /// Data contract for returning access token in response.
    /// </summary>
    internal class TokenResponseBody
    {
        public string Token { get; set; }
    }
}