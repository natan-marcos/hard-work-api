using System.ComponentModel.DataAnnotations;
using HardWorkAPI.Models;

namespace HardWorkAPI.DTOs
{
    public class UpdateUserDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [EmailAddress, MaxLength(120)]
        public string? Email { get; set; }
        
        public string? PasswordHash { get; set; }

        [Phone, MaxLength(20)]
        public string? Phone { get; set; }

        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid role.")]
        public UserRole? Role { get; set; }
    }
}
