using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Admin
{
    public class AddUsersModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Users> _passwordHasher;

        public AddUsersModel(AppDbContext context, IPasswordHasher<Users> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public new Users User { get; set; } = new()
        {
            SurName = string.Empty,
            Name = string.Empty,
            Patronomic = string.Empty,
            Login = string.Empty,

            Phone = string.Empty,
            Email = string.Empty
        };

        [BindProperty]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = string.Empty; // временное свойство для ввода пароля

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

            ModelState.Remove("User.PasswordHash");

            if (User.IdRole == 0)
                ModelState.AddModelError("User.IdRole", "Выберите роль");

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Хэширование пароля перед сохранением
            User.PasswordHash = _passwordHasher.HashPassword(User, Password);
            User.CreationDate = DateTime.UtcNow;

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/AdminUsers");
        }
    }
}
