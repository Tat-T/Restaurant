using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace MyRazorApp.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!Request.Cookies.ContainsKey("AdminAuth"))
            {
                return RedirectToPage("/Admin/Login");
            }
            return Page();
        }
    }
}
