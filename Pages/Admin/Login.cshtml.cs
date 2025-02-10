using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyRazorApp.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        
        public LoginModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public required string Username { get; set; }

        [BindProperty]
        public required string Password { get; set; }

        public required string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var hashedPassword = ComputeSha256Hash(Password);
            var admin = _db.AdminUsers.FirstOrDefault(a => a.Username == Username && a.PasswordHash == hashedPassword);

            if (admin != null)
            {
                HttpContext.Session.SetString("Admin", Username);
                return RedirectToPage("/Admin/Index");
            }
            
            ErrorMessage = "Неверный логин или пароль.";
            return Page();
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
