using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Data;
using System.Security.Cryptography;
using System.Text;

namespace MyRazorApp.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Username == Username);
            if (admin == null || !VerifyPassword(Password, admin.PasswordHash))
            {
                ErrorMessage = "Неверное имя пользователя или пароль";
                return Page();
            }

            Response.Cookies.Append("AdminAuth", "true");
            return RedirectToPage("/Admin/Index");
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hashString == storedHash;
        }
    }
}
