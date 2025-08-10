using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public AdminUsersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Users> UsersList { get; set; } = new();

        public async Task OnGetAsync()
        {
            UsersList = await _context.Users
                .Include(u => u.UserRole) // Чтобы вытянуть название роли
                .ToListAsync();
        }
    }
}
