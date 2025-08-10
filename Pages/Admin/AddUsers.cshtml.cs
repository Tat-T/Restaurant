using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Pages.Admin
{
    public class AddUsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public AddUsersModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Users User { get; set; } = new()
        {
            SurName = string.Empty,
            Name = string.Empty,
            Patronomic = string.Empty,
            Login = string.Empty,
            Password = string.Empty,
            Phone = string.Empty,
            Email = string.Empty
        };
        public List<SelectListItem> RoleList { get; set; } = new();

        public async Task OnGetAsync()
        {
            RoleList = await _context.UserRoles
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name ?? "Без названия"
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            User.CreationDate = DateTime.UtcNow;
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/AdminUsers");
        }
    }
}
