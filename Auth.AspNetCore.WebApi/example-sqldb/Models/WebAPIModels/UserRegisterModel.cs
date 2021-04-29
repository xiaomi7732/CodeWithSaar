using System.ComponentModel.DataAnnotations;

namespace JWT.Example.WithSQLDB
{
    public class UserRegisterModel
    {
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        // More for others
    }
}