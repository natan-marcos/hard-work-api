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
                return BadRequest(new { message = "Invalid user ID" });

            var workouts = await _context.Workouts
                .Where(w => w.TrainerId == userIdSafe)
                .ToListAsync();

            return Ok(workouts);
        }

        [HttpGet("{id}", Name = "GetWorkoutById")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetById(long id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!long.TryParse(userId, out var userIdLong))
                return BadRequest(new { message = "Invalid user ID" });

            Workout? workout = await _context.Workouts.FindAsync(id);
            if (workout == null) return NotFound();

            if (workout.TrainerId != userIdLong && userRole != "Admin")
                return Forbid();

            return Ok(workout);
        }

        [HttpPost]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Create(CreateWorkoutDto workoutDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!long.TryParse(userId, out var userIdSafe))
                return BadRequest(new { message = "Invalid user ID" });

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

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Update(long id, UpdateWorkoutDto workoutDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!long.TryParse(userId, out var userIdLong))
                return BadRequest(new { message = "Invalid user ID" });

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
                return NotFound(new { message = "Workout not found" });

            if (workout.TrainerId != userIdLong && userRole != "Admin")
                return Forbid();

            workout.Name = workoutDto.Name ?? workout.Name;
            workout.Description = workoutDto.Description ?? workout.Description;

            _context.Workouts.Update(workout);
            await _context.SaveChangesAsync();

            return Ok(workout);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!long.TryParse(userId, out var userIdLong))
                return BadRequest(new { message = "Invalid user ID" });

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
                return NotFound(new { message = "Workout not found" });

            if (workout.TrainerId != userIdLong && userRole != "Admin")
                return Forbid();

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // WORKOUTS EXERCISES
        [HttpPost("{id}/exercises")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> AddExerciseToWorkout(long id, AddExerciseToWorkoutDto exerciseDto)
        {
            // get user info from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!long.TryParse(userId, out var userIdLong))
                return BadRequest(new { message = "Invalid user ID" });

            // get workout from db
            var workout = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .FirstOrDefaultAsync(w => w.Id == id);

            // validate
            if (workout == null)
                return NotFound(new { message = "Workout not found" });

            // check if user is owner or admin
            if (workout.TrainerId != userIdLong && userRole != "Admin")
                return Forbid();

            // check if exercise id is valid
            var exercise = await _context.Exercises.FindAsync(exerciseDto.ExerciseId);
            if (exercise == null)
                return NotFound(new { message = "Exercise not found" });

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
                return Conflict(new { message = "This exercise is already added to the workout" });
            }

            return Created();
        }

        [HttpDelete("{workoutId}/exercises/{exerciseId}")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> RemoveExerciseFromWorkout(long workoutId, long exerciseId)
        {
            // get user info from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // parse userId
            if (!long.TryParse(userId, out var userIdLong))
                return BadRequest(new { message = "Invalid user ID" });

            // check if workout exists and belongs to the user (if not admin)
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.TrainerId == userIdLong);
            if (workout == null && userRole != "Admin")
                return NotFound(new { message = "Workout not found or does not belong to the user" });

            // get the relation
            var workoutExercise = await _context.WorkoutExercises
                .FirstOrDefaultAsync(we => we.WorkoutId == workoutId && we.ExerciseId == exerciseId);

            if (workoutExercise == null)
                return NotFound(new { message = "Exercise not found in this workout" });

            // remove the relation
            _context.WorkoutExercises.Remove(workoutExercise);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
