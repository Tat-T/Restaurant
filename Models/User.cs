using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
}
