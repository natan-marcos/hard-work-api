using System.ComponentModel.DataAnnotations;

namespace HardWorkAPI.DTOs
{
    public class CreateWorkoutDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
