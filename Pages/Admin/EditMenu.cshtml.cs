using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditMenuModel : PageModel
    {
        public void OnGet() { }
    }
}
