using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

public class LoginModel : PageModel
{
    [BindProperty]
    public required string Email { get; set; }
    [BindProperty]
    public required string Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        // Здесь должна быть проверка учетных данных пользователя
        if (Email == "user@example.com" && Password == "password")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Account/Logout");
        }

        ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
        return Page();
    }
}
