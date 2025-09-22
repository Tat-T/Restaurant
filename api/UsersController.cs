using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Api
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // Получить список всех ролей
        // GET: api/users/roles
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Select(r => new { id = r.Id, name = r.Name })
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRole)
                .AsNoTracking()
                .Select(u => new 
                {
                    u.Id,
                    u.SurName,
                    u.Name,
                    u.Patronomic,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.Birthdate,
                    u.CreationDate,
                    u.IsActive,
                    UserRole = u.UserRole != null ? new { u.UserRole.Id, u.UserRole.Name } : null
                })
                .ToListAsync();

            return Ok(users);
        }

        // Получить пользователя по Id
        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRole)
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new 
                {
                    u.Id,
                    u.SurName,
                    u.Name,
                    u.Patronomic,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.Birthdate,
                    u.CreationDate,
                    u.IsActive,
                    UserRole = u.UserRole != null ? new { u.UserRole.Id, u.UserRole.Name } : null
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound(new { message = "Пользователь не найден" });

            return Ok(user);
        }

        // Добавить нового пользователя
        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User
            {
                SurName = model.SurName,
                Name = model.Name,
                Patronomic = model.Patronomic,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Birthdate = model.Birthdate,
                IdRole = model.IdRole,
                IsActive = model.IsActive,
                CreationDate = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь добавлен", user.Id });
        }

        // Обновить пользователя
        // PUT: api/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound(new { message = "Пользователь не найден" });

            // обновляем данные
            user.SurName = model.SurName;
            user.Name = model.Name;
            user.Patronomic = model.Patronomic;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Birthdate = model.Birthdate;
            user.IdRole = model.IdRole;
            user.IsActive = model.IsActive;

            // если новый пароль передан — обновляем
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь обновлён", user.Id });
        }

        // Удалить пользователя
        // DELETE: api/users/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound(new { message = "Пользователь не найден" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь удалён", user.Id });
        }
    }

    // DTO для создания
    public class AddUserDto
    {
        public string SurName { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Patronomic { get; set; }
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthdate { get; set; }
        public int IdRole { get; set; }
        public bool IsActive { get; set; }
    }

    // DTO для обновления
    public class UpdateUserDto
    {
        public string SurName { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Patronomic { get; set; }
        public string UserName { get; set; } = "";
        public string? NewPassword { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthdate { get; set; }
        public int IdRole { get; set; }
        public bool IsActive { get; set; }
    }
}
