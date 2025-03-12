using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
            new AuthenticationProperties { ExpiresUtc = DateTime.UtcNow, IsPersistent = false });

        Response.Cookies.Delete(".AspNetCore.Cookies"); 
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
            new AuthenticationProperties { ExpiresUtc = DateTime.UtcNow, IsPersistent = false });

        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToPage("/Index");
    }
}
