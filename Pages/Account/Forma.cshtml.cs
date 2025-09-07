using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FormaModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public FormaModel(AppDbContext context, IPasswordHasher<User> passwordHasher)
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
        // Поиск пользователя в БД
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == Email);

        if (user != null && user.IsActive)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);

            if (result == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.IdRole == 1 ? "Admin" : "User"),
                    new Claim("FullName", $"{user.SurName} {user.Name} {user.Patronomic}"),
                    new Claim("Phone", user.PhoneNumber ?? ""),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                if (user.IdRole == 1)
                    return RedirectToPage("/Account/AdminPanel");
                else
                    return RedirectToPage("/Account/Index");
            }
        }

        ModelState.AddModelError(string.Empty, "Неверный логин, пароль или аккаунт не активирован.");
        return Page();
    }
}
