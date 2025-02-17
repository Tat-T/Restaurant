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
        if (Email == "user@admin.ru" && Password == "123456")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Account/Index2");
        }

        ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
        return Page();
    }
}
