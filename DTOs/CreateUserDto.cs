using System.ComponentModel.DataAnnotations;
using HardWorkAPI.Models;

namespace HardWorkAPI.DTOs
{
    public class CreateUserDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(120), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid role.")]
        public UserRole Role { get; set; } = UserRole.Student;

        [Phone, MaxLength(20)]
        public string? Phone { get; set; }
    }
}
