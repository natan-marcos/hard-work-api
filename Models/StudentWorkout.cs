using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardWorkAPI.Models;

public class StudentWorkout
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("Workout")]
    public long WorkoutId { get; set; }
    public Workout Workout { get; set; }

    [ForeignKey("Student")]
    public long StudentId { get; set; }
    public User Student { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
