using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;

        public EditUserModel(UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public User? User { get; set; }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? SelectedRoleId { get; set; }

        public List<SelectListItem> RoleList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (User == null) return NotFound();

            // Загружаем роли
            RoleList = await _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name ?? "Без названия"
                })
                .ToListAsync();

            // Текущая роль пользователя
            var roles = await _userManager.GetRolesAsync(User);
            if (roles.Any())
            {
                var role = await _roleManager.FindByNameAsync(roles.First());
                SelectedRoleId = role?.Id.ToString();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User == null) return BadRequest();

            var existingUser = await _userManager.FindByIdAsync(User.Id.ToString());
            if (existingUser == null) return NotFound();

            // Обновляем поля
            existingUser.SurName = User.SurName;
            existingUser.Name = User.Name;
            existingUser.Patronomic = User.Patronomic;
            existingUser.Email = User.Email;
            existingUser.PhoneNumber = User.PhoneNumber;
            existingUser.Birthdate = User.Birthdate;
            existingUser.IsActive = User.IsActive;

            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await LoadRolesAsync();
                return Page();
            }

            // Обновляем пароль, если указано
            if (!string.IsNullOrEmpty(NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var passwordResult = await _userManager.ResetPasswordAsync(existingUser, token, NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    await LoadRolesAsync();
                    return Page();
                }
            }

            // Обновляем роль
            if (!string.IsNullOrEmpty(SelectedRoleId))
            {
                var currentRoles = await _userManager.GetRolesAsync(existingUser);
                await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);

                var role = await _roleManager.FindByIdAsync(SelectedRoleId);
                if (role != null)
                    await _userManager.AddToRoleAsync(existingUser, role.Name!);
            }

            return RedirectToPage("/Admin/AdminUsers");
        }

        private async Task LoadRolesAsync()
        {
            RoleList = await _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name ?? "Без названия"
                })
                .ToListAsync();
        }
    }
}
