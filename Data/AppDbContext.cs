using HardWorkAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HardWorkAPI.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
    public DbSet<StudentWorkout> StudentWorkouts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ProgressPhoto> ProgressPhotos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // UNIQUE constraint: um exercício só pode aparecer 1x por treino
        modelBuilder.Entity<WorkoutExercise>()
            .HasIndex(we => new { we.WorkoutId, we.ExerciseId })
            .IsUnique();

        // UNIQUE constraint: um treino só pode ser atribuído 1x a um aluno
        modelBuilder.Entity<StudentWorkout>()
            .HasIndex(sw => new { sw.WorkoutId, sw.StudentId })
            .IsUnique();

        // Relacionamento: Um usuário (aluno) pode ter vários pagamentos
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Student)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
