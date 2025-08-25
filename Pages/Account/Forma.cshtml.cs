using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class FormaModel : PageModel
{
     private readonly AppDbContext _context;

        public FormaModel(AppDbContext context)
        {
            _context = context;
        }

    [BindProperty]
    public required string Email { get; set; }
    [BindProperty]
    public required string Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
         // Поиск пользователя в БД
            var user = await _context.Users
                .Include(u => u.UserRole) 
                .FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == Password);

            if (user != null && user.IsActive)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.IdRole == 1 ? "Admin" : "User"), // Добавляем роль
                    new Claim("FullName", $"{user.SurName} {user.Name} {user.Patronomic}"),
                    new Claim("Phone", user.Phone ?? ""),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity));

            if (User.IsInRole("Admin"))
            {
                
                return RedirectToPage("/Account/AdminPanel"); 
                
            }
            else

                return RedirectToPage("/Account/Index"); // Перенаправление после входа
            }

            ModelState.AddModelError(string.Empty, "Неверный логин, пароль или аккаунт не активирован.");
            return Page();
    }
}
