using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardWorkAPI.Models;

public class Workout
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("Trainer")]
    public long TrainerId { get; set; }
    public User Trainer { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    public ICollection<StudentWorkout> StudentWorkouts { get; set; } = new List<StudentWorkout>();
}

