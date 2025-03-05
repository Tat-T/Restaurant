using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(15)]
        public string? Name { get; set; }

        [MaxLength(15)]
        public string? Descriptions { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
