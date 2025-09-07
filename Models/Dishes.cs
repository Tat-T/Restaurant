using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Dishes
{
    [Key]
    public int DishID { get; set; }

    [Required]
    [StringLength(255)]
    public string DishName { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public string? DishImage { get; set; }

    // Навигация к связке с ингредиентами
    public List<DishIngredients> DishIngredients { get; set; } = new();
}
