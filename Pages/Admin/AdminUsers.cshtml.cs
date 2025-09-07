using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRazorApp.Pages.Admin
{
    // [Authorize(Roles = "ADMIN")]
    public class AdminUsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public AdminUsersModel(AppDbContext context)
        {
            _context = context;
        }

        // Список пользователей
        public List<User> UsersList { get; set; } = new();

        // Словарь Id -> Название роли
        public Dictionary<int, string> RolesDict { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Загружаем пользователей
                   UsersList = await _context.Users
            .Include(u => u.UserRole)
            .AsNoTracking()
            .ToListAsync();


            // Загружаем роли
            RolesDict = await _context.Roles
                .AsNoTracking()
                .ToDictionaryAsync(r => r.Id, r => r.Name ?? "Без названия");
        }
    }
}
