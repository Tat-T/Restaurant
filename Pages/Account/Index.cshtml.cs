using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MyRazorApp.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!Request.Cookies.ContainsKey("AdminAuth"))
            {
                return RedirectToPage("/Account/Index");
            }
            return Page();
        }
    }
}
