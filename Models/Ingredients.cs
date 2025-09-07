using System.ComponentModel.DataAnnotations;

public class Ingredients
{
    [Key]
    public int IngredientID { get; set; }

    [Required]
    [StringLength(255)]
    public string IngredientName { get; set; } = string.Empty;

    // Навигация к связке с блюдами
    public List<DishIngredients> DishIngredients { get; set; } = new();
}
