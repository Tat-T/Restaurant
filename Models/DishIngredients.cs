using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Administrator")]
public class DishIngredients {
    public int DishID { get; set; }
    public required Dishes Dishes { get; set; }

    public int IngredientID { get; set; }
    public required Ingredients Ingredients { get; set; }

}
