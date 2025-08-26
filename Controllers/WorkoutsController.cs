using System.Security.Claims;
using HardWorkAPI.Data;
using HardWorkAPI.DTOs;
using HardWorkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HardWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkoutsController(AppDbContext context)
        {
            _context = context;
        }

        // WORKOUTS BASICS

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            List<Workout> workouts = await _context.Workouts.ToListAsync();

            return Ok(workouts);
        }

        [HttpGet]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetMyWorkouts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!long.TryParse(userId, out var userIdSafe))
                return BadRequest("Invalid user ID");

            var workouts = await _context.Workouts
                .Where(w => w.TrainerId == userIdSafe)
                .ToListAsync();

            return Ok(workouts);
        }

        [HttpGet("{id}", Name = "GetWorkoutById")]
        public async Task<IActionResult> GetById(long id)
        {
            // TODO adicionar autorization
            Workout? workout = await _context.Workouts.FindAsync(id);
            if (workout == null) return NotFound();

            return Ok(workout);
        }

        [HttpPost]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Create(CreateWorkoutDto workoutDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!long.TryParse(userId, out var userIdSafe))
                return BadRequest("Invalid user ID");

            Workout workout = new Workout
            {
                TrainerId = userIdSafe,
                Name = workoutDto.Name,
                Description = workoutDto.Description
            };

            await _context.Workouts.AddAsync(workout);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetWorkoutById", new { id = workout.Id }, workout);
        }

        // WORKOUTS EXERCISES

        [HttpPost("{id}/exercises")]
        public async Task<IActionResult> AddExerciseToWorkout(long id, AddExerciseToWorkoutDto exerciseDto)
        {
            // get workout from db
            var workout = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .FirstOrDefaultAsync(w => w.Id == id);

            // validate
            if (workout == null)
                return NotFound("Workout not found");

            // check if exercise id is valid
            var exercise = await _context.Exercises.FindAsync(exerciseDto.ExerciseId);
            if (exercise == null)
                return NotFound("Exercise not found");

            var workoutExercise = new WorkoutExercise
            {
                WorkoutId = workout.Id,
                ExerciseId = exercise.Id,
                Sets = exerciseDto.Sets,
                Reps = exerciseDto.Reps,
                RestTime = exerciseDto.RestTime ?? 60 // default rest time 60s
            };

            workout.WorkoutExercises.Add(workoutExercise);
            try
            {
                await _context.SaveChangesAsync();
            } 
            catch (DbUpdateException ex)
            {
                return Conflict("This exercise is already added to the workout");
            }

            return Ok("The exercise was added successfully");
        }
    }
}
