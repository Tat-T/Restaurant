using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(15)]
        public string? SurName { get; set; }

        [MaxLength(15)]
        public string? Name { get; set; }

        [MaxLength(15)]
        public string? Patronomic { get; set; }

        [Required]
        [MaxLength(15)]
        public string Login { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? Password { get; set; }

        [MaxLength(15)]
        public string? ViewName { get; set; }

        [MaxLength(12)]
        public string? Phone { get; set; }

        [MaxLength(25)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserRole")]
        public int IdRole { get; set; }
        public UserRole? UserRole { get; set; } // Навигационное свойство

        public bool IsActive { get; set; } = false;
    }
}
