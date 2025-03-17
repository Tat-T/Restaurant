using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Administrator")]
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

    public List<DishIngredients> DishIngredients { get; set; } = new();
    public string? DishImage { get; internal set; }
}