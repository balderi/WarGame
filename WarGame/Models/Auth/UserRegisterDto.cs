using System.ComponentModel.DataAnnotations;

namespace WarGame.Models.Auth
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Max length: 50")]
        [MinLength(2, ErrorMessage = "Min length: 2")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(255, ErrorMessage = "Max length: 255")]
        [MinLength(8, ErrorMessage = "Min length: 8")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Not a match")]
        public string PasswordConfirmed { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Compare("Email", ErrorMessage = "Not a match")]
        public string EmailConfirmed { get; set; } = string.Empty;
    }
}
