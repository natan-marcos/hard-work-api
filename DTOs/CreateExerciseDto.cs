using System.ComponentModel.DataAnnotations;

namespace HardWorkAPI.DTOs;
public class CreateExerciseDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required, MaxLength(50)]
    public string muscleGroup { get; set; } = string.Empty;
    [Required, Url]
    public string imgeUrl { get; set; } = string.Empty;
}