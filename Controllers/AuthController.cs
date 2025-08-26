using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HardWorkAPI.Data;
using HardWorkAPI.DTOs;
using HardWorkAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HardWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _hasher;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _hasher = new PasswordHasher<User>();
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            if (_context.Users.Any(u => u.Email == userDto.Email))
                return BadRequest(new { message = "Email already registered" });

            User user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Role = userDto.Role,
                Phone = userDto.Phone
            };

            user.PasswordHash = _hasher.HashPassword(user, userDto.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetUserById", new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUserDto userDto)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == userDto.Email);
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Invalid credentials" });

            // Pegando a key, issuer, audience e tempo de expiração do appsettings.json
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expireHours = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);

            // Gerar o token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(expireHours),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { Token = jwt });
        }
    }
}
