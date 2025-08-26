using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardWorkAPI.Models;

public class WorkoutExercise
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("Workout")]
    public long WorkoutId { get; set; }
    public Workout Workout { get; set; }

    [ForeignKey("Exercise")]
    public long ExerciseId { get; set; }
    public Exercise Exercise { get; set; }

    [Required]
    public int Sets { get; set; }

    [Required]
    public int Reps { get; set; }

    public int? RestTime { get; set; } // seconds
}
