using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyRazorApp.Data;
using MyRazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Restaurant.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public class RegisterRequest
        {
            public string SurName { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Patronomic { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                SurName = model.SurName,
                Name = model.Name,
                Patronomic = model.Patronomic,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                CreationDate = DateTime.UtcNow,
                IdRole = 3,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(new { message = "Регистрация успешна", redirectUrl = "/Zakaz" });
        }
    
         // --- Выход ---
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return Ok(new { message = "Вы вышли из системы" });
        }

        // --- Мои заказы ---
       [HttpGet("reservations")]
       [Authorize]
       public async Task<IActionResult> GetMyReservations()
       {
           var userEmail = User.Identity?.Name;
           if (string.IsNullOrEmpty(userEmail))
               return Unauthorized();
       
           var reservations = await _context.Reservations
               .Where(r => r.Email == userEmail)
               .AsNoTracking()
               .Select(r => new
               {
                   id = r.Id,
                   date = r.ReservationDate,
                   time = r.ReservationTime,
                   guests = r.Guests,
                   comment = r.Message
               })
               .ToListAsync();
       
           return Ok(reservations);
       }
}
}