using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyRazorApp.Models;

namespace Restaurant.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !user.IsActive)
                return Unauthorized(new { message = "Неверный логин или аккаунт не активирован." });

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Неверный пароль." });

            return Ok(new { message = "Успешный вход", role = user.IdRole == 1 ? "Admin" : "User" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return Ok(new { message = "Вы вышли из системы" });
        }
    }
}
