using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

public class DishIngredients
{
    public int DishID { get; set; }
    public Dishes Dishes { get; set; }

    public int IngredientID { get; set; }
    public Ingredients Ingredients { get; set; }
}