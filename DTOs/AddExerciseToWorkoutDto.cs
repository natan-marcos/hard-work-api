using System.ComponentModel.DataAnnotations;

namespace HardWorkAPI.DTOs
{
    public class AddExerciseToWorkoutDto
    {
        [Required]
        public long ExerciseId { get; set; }

        [Required]
        public int Sets { get; set; }

        [Required]
        public int Reps { get; set; } // repetitions

        public int? RestTime { get; set; } // seconds
    }
}