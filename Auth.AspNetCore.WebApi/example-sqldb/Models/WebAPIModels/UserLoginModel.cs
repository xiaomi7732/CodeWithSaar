using System.ComponentModel.DataAnnotations;

namespace JWT.Example.WithSQLDB
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}