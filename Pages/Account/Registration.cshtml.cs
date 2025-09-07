using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Pages.Account
{
    public class RegistrationModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public RegistrationModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required, MaxLength(15)]
            public string SurName { get; set; } = string.Empty;

            [Required, MaxLength(15)]
            public string Name { get; set; } = string.Empty;

            [Required, MaxLength(15)]
            public string Patronomic { get; set; } = string.Empty;

            [Required, MaxLength(50)]
            public string UserName { get; set; } = string.Empty;

            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required, Phone]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new User
            {
                SurName = Input.SurName,
                Name = Input.Name,
                Patronomic = Input.Patronomic,
                UserName = Input.UserName,
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                CreationDate = DateTime.UtcNow,
                IdRole = 5,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Zakaz");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}
