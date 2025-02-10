using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class AdminUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }  // Пароль будет храниться в зашифрованном виде
    }
}
