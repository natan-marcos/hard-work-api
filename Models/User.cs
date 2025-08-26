using System.ComponentModel.DataAnnotations;

namespace HardWorkAPI.Models;

public class User
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Student;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<Workout> WorkoutsCreated { get; set; } = new List<Workout>();
    public ICollection<StudentWorkout> StudentWorkouts { get; set; } = new List<StudentWorkout>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<ProgressPhoto> ProgressPhotos { get; set; } = new List<ProgressPhoto>();
}

public enum UserRole
{
    Student = 0,
    Trainer = 1,
    Admin = 2
}
