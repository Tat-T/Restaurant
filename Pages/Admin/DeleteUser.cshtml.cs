using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Admin
{
    public class DeleteUserModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteUserModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public new User? User { get; set; }= new()
        {
            SurName = string.Empty,
            Name = string.Empty,
            Patronomic = string.Empty,
            UserName = string.Empty,
            PasswordHash = string.Empty,
            PhoneNumber = string.Empty,
            Email = string.Empty
        };

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _context.Users
                .Include(u => u.IdRole)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/AdminUsers");
        }
    }
}
