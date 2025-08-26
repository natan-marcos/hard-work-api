using System.ComponentModel.DataAnnotations;

namespace HardWorkAPI.DTOs
{
    public class LoginUserDto
    {
        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
