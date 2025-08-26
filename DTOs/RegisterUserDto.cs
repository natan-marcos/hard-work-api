using System.ComponentModel.DataAnnotations;
using HardWorkAPI.Models;

namespace HardWorkAPI.DTOs
{
    public class RegisterUserDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.Student;

        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}