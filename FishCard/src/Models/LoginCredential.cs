using System.ComponentModel.DataAnnotations;

namespace CodeWithSaar.FishCard.Auth;

public class LoginCredential
{
    [Required]
    public string UserName { get; set; } = default!;
    
    [Required]
    public string Password { get; set; } = default!;
}