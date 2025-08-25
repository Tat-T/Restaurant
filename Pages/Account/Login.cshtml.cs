using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

public class LoginModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<MyRazorApp.Models.Users> _passwordHasher;

    public LoginModel(AppDbContext context, IPasswordHasher<MyRazorApp.Models.Users> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public required string Email { get; set; }

    [BindProperty]
    public required string Password { get; set; }

   public async Task<IActionResult> OnPostAsync()
{
    var user = await _context.Users
        .Include(u => u.UserRole)
        .FirstOrDefaultAsync(u => u.Email == Email);

    if (user != null && user.IsActive)
    {
        PasswordVerificationResult result = PasswordVerificationResult.Failed;

        try
        {
            // стандартная проверка хэша
            result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);
        }
        catch (FormatException)
        {
            // если в базе лежит обычный пароль (старый формат)
            if (user.PasswordHash == Password)
            {
                // перехэшируем и сохраним
                user.PasswordHash = _passwordHasher.HashPassword(user, Password);
                await _context.SaveChangesAsync();
                result = PasswordVerificationResult.Success;
            }
        }

        if (result == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.IdRole == 1 ? "Admin" : "Guest"),
                new Claim("FullName", $"{user.SurName} {user.Name} {user.Patronomic}"),
                new Claim("Phone", user.Phone ?? ""),
                new Claim("UserId", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return user.IdRole == 1 
                ? RedirectToPage("/Admin/AdminPanel") 
                : RedirectToPage("/Account/Index");
        }
    }

        ModelState.AddModelError(string.Empty, "Неверный логин, пароль или аккаунт не активирован.");
        return Page();
    }
}
