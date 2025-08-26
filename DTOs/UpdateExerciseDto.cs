using System.ComponentModel.DataAnnotations;

namespace HardWorkAPI.DTOs
{
    public class UpdateExerciseDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? muscleGroup { get; set; } = string.Empty;
        
        [Url]
        public string? imgeUrl { get; set; } = string.Empty;
    }
}
