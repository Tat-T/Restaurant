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
        public required string SurName { get; set; }

        [MaxLength(15)]
        public required string Name { get; set; }

        [MaxLength(15)]
        public required string Patronomic { get; set; }

        [Required]
        [MaxLength(15)]
        public required string Login { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(12)]
        public required string Phone { get; set; }

        [MaxLength(25)]
        public required string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserRole")]
        public int IdRole { get; set; }
        public UserRole? UserRole { get; set; }

        public bool IsActive { get; set; } = false;
    }
}
