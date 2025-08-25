using Microsoft.AspNetCore.Identity;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Services
{
    public class PasswordMigration
    {
        public static async Task RunAsync(AppDbContext context, IPasswordHasher<Users> passwordHasher)
        {
            var users = context.Users.ToList();
            bool changed = false;

            foreach (var user in users)
            {
                // Проверка: Identity-хэши обычно начинаются с "AQAAAA"
                if (!string.IsNullOrEmpty(user.PasswordHash) && !user.PasswordHash.StartsWith("AQAAAA"))
                {
                    user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);
                    changed = true;
                }
            }

            if (changed)
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
