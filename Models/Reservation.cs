using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Reservation
{
    [Key] // Указывает, что Id — это первичный ключ
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress] // Проверка на корректность email
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone] // Проверка на корректный номер телефона
    public string Phone { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)] // Указывает, что это дата
    public DateTime ReservationDate { get; set; }

    [Required]
    [DataType(DataType.Time)] // Указывает, что это время
    public TimeSpan ReservationTime { get; set; }

    [Required]
    [Range(1, 100)] // Минимальное и максимальное число гостей
    public int Guests { get; set; }
    public string? Message { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоматическое создание времени
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
