using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter a name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must enter an email.")]
        [EmailAddress(ErrorMessage = "You must enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must enter a password.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
