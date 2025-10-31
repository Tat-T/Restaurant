using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Reservation
{
    [Key] // Первичный ключ
    public int Id { get; set; }

    [Required]
    [StringLength(100)] // Ограничим имя по длине
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress] // Проверка на email
    public string Email { get; set; } = string.Empty;

    [Required]
    // [Phone] // Проверка на телефон
    public string Phone { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime ReservationDate { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan ReservationTime { get; set; }

    [Required]
    [Range(1, 50)] // допустим, максимум 50 гостей
    public int Guests { get; set; }

    [MaxLength(500)] // ограничение длины заметки
    public string? Message { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
