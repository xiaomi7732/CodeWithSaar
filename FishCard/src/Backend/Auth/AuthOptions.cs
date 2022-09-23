namespace CodeWithSaar.FishCard.Auth;

public class AuthOptions
{
    public string TokenSigningKey { get; set; } = default!;
    public string Issuer { get; set; } = "CodeWithSaar";
    public string Audience { get; set; } = "Fishcard";
    public TimeSpan ExpiryTimeSpan { get; set; } = TimeSpan.FromMinutes(5);
}