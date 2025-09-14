using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;

public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    [Required(ErrorMessage = "Введите Email")]
    [EmailAddress(ErrorMessage = "Некорректный Email")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page(); // Ошибки попадут в asp-validation-for
        }

        var user = await _userManager.FindByEmailAsync(Email);
        if (user != null && user.IsActive)
        {
            var result = await _signInManager.PasswordSignInAsync(user, Password, false, false);
            if (result.Succeeded)
            {
                return user.IdRole == 1
                    ? RedirectToPage("/Admin/AdminPanel")
                    : RedirectToPage("/Account/Index");
            }
        }

        ModelState.AddModelError(string.Empty, "Неверный логин, пароль или аккаунт не активирован.");
        return Page();
    }
}
