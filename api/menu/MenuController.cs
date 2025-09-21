using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

namespace Restaurant.Api.Menu
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {
            var dishes = await _context.Dishes
                .Select(d => new
                {
                    d.DishID,
                    d.DishName,
                    d.Price,
                    DishImage = string.IsNullOrEmpty(d.DishImage) ? "/image/dishes/no-photo.jpg" : d.DishImage,
                    Ingredients = _context.DishIngredients
                        .Where(di => di.DishID == d.DishID)
                        .Join(_context.Ingredients,
                              di => di.IngredientID,
                              i => i.IngredientID,
                              (di, i) => i.IngredientName)
                        .ToList()
                })
                .ToListAsync();

            return Ok(dishes);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
                return NotFound();

            // Удаляем связи DishIngredients
            var links = _context.DishIngredients.Where(di => di.DishID == id);
            _context.DishIngredients.RemoveRange(links);

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Блюдо удалено" });
        }
    }
}
