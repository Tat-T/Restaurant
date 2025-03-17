using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

[Authorize(Roles = "Administrator")]

public class Ingredients
{
    [Key]
    public int IngredientID { get; set; }

    [Required]
    [StringLength(255)]
    public string IngredientName { get; set; } = string.Empty;

    public List<DishIngredients> DishIngredients { get; set; } = new();
}
