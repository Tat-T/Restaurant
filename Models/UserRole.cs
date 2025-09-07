using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class UserRole : IdentityRole<int>
    {
        [MaxLength(15)]
        public string? Descriptions { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
