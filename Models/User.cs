using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Models
{
    public class User : IdentityUser<int>
    {
        [MaxLength(15)]
        public string SurName { get; set; } = string.Empty;

        [MaxLength(15)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(15)]
        public string Patronomic { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = false;

        public int IdRole { get; set; }

        public UserRole? UserRole { get; set; }
    }
}
