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
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
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
