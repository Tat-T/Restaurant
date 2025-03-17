using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Administrator")]
public class DishViewModel
{
    public int DishID { get; set; }
    public string DishName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string DishImage { get; set; } = string.Empty;
    public List<string> Ingredients { get; set; } = new();
}