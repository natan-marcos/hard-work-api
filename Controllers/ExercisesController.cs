using HardWorkAPI.Data;
using HardWorkAPI.DTOs;
using HardWorkAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HardWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExercisesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<Exercise> exercises = await _context.Exercises.ToListAsync();
            return Ok(exercises);
        }

        [HttpGet("{id}", Name = "GetExerciseById")]
        public async Task<IActionResult> GetById(long id)
        {
            Exercise? exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null) return NotFound(new { message = "Exercise not found" });
            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExerciseDto exerciseDto)
        {
            Exercise exercise = new Exercise
            {
                Name = exerciseDto.Name,
                Description = exerciseDto.Description,
                MuscleGroup = exerciseDto.muscleGroup,
                ImageUrl = exerciseDto.imgeUrl
            };

            await _context.Exercises.AddAsync(exercise);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetExerciseById", new { id = exercise.Id }, exercise);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UpdateExerciseDto exerciseDto)
        {
            Exercise? exercise = _context.Exercises.Find(id);
            if (exercise == null) return NotFound(new { message = "Exercise not found" });

            exercise.Name = exerciseDto.Name ?? exercise.Name;
            exercise.Description = exerciseDto.Description ?? exercise.Description;
            exercise.MuscleGroup = exerciseDto.muscleGroup ?? exercise.MuscleGroup;
            exercise.ImageUrl = exerciseDto.imgeUrl ?? exercise.ImageUrl;
            
            _context.Exercises.Update(exercise);
            await _context.SaveChangesAsync();
            return Ok(exercise);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            Exercise? exercise = _context.Exercises.Find(id);
            if (exercise == null) return NotFound(new { message = "Exercise not found" });
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
