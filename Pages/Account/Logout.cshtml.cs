using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        // Очистка аутентификационных куки
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Очистка сессии
        HttpContext.Session.Clear();

        // Перенаправление после выхода
        return RedirectToPage("/Account/Login");
    }
}
