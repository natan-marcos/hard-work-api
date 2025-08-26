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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            List<User> users = await _context.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            // get user id from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // convert to long
            if (!long.TryParse(userId, out var userIdSafe))
                return BadRequest(new { message = "Invalid user ID" });

            // get from db
            var user = await _context.Users
                .Where(u => u.Id == userIdSafe)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role,
                    u.Phone
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetById(long id) { 
            User? user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateUserDto userDto)
        {

            User user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = userDto.PasswordHash,
                Role = userDto.Role,
                Phone = userDto.Phone
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetUserById", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(long id, UpdateUserDto userDto)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            user.Name = userDto.Name ?? user.Name;
            user.Email = userDto.Email ?? user.Email;
            user.PasswordHash = userDto.PasswordHash ?? user.PasswordHash;
            user.Role = userDto.Role ?? user.Role;
            user.Phone = userDto.Phone ?? user.Phone;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            User? user = _context.Users.Find(id);
            if (user == null) return NotFound(new { message = "User not found" });
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
