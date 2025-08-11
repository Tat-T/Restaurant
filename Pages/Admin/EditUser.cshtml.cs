using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Admin
{
    public class EditUserModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditUserModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
         public new Users User { get; set; } = new()
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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (User == null)
            {
                return NotFound();
            }

            RoleList = await _context.UserRoles
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name ?? "Без названия"
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesAsync();
                return Page();
            }

            var existingUser = await _context.Users.FindAsync(User.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Обновляем поля
            existingUser.SurName = User.SurName;
            existingUser.Name = User.Name;
            existingUser.Patronomic = User.Patronomic;
            existingUser.Login = User.Login;
            existingUser.Password = User.Password;
            existingUser.Email = User.Email;
            existingUser.Phone = User.Phone;
            existingUser.Birthdate = User.Birthdate;
            existingUser.IdRole = User.IdRole;
            existingUser.IsActive = User.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/AdminUsers");
        }

        private async Task LoadRolesAsync()
        {
            RoleList = await _context.UserRoles
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name ?? "Без названия"
                })
                .ToListAsync();
        }
    }
}
