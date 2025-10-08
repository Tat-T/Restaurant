using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

namespace Restaurant.Api
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MenuController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ---------------- GET ----------------
        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {
            var dishes = await _context.Dishes
                .Select(d => new
                {
                    d.DishID,
                    d.DishName,
                    d.Price,
                    DishImage = string.IsNullOrEmpty(d.DishImage) ? "/images/dishes/no-photo.jpg" : d.DishImage,
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

        // ---------------- GET BY ID ----------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDish(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredients)
                .FirstOrDefaultAsync(d => d.DishID == id);

            if (dish == null) return NotFound();

            return Ok(new
            {
                dish.DishID,
                dish.DishName,
                dish.Price,
                dish.DishImage,
                Ingredients = dish.DishIngredients.Select(di => di.Ingredients.IngredientName).ToList()
            });
        }

        // ---------------- CREATE ----------------
        [HttpPost]
        public async Task<IActionResult> CreateDish([FromForm] DishInputModel model, IFormFile? images)
        {
            string imagePath = "/images/no-photo.jpg";

            if (images != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(images.FileName)}";
                var filePath = Path.Combine(_env.WebRootPath, "images", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await images.CopyToAsync(stream);

                imagePath = "/images/" + fileName;
            }

            var dish = new Dishes
            {
                DishName = model.DishName,
                Price = model.Price,
                DishImage = imagePath
            };

            // ингредиенты
            if (!string.IsNullOrWhiteSpace(model.IngredientNames))
            {
                var ingredients = model.IngredientNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                dish.DishIngredients = new List<DishIngredients>();

                foreach (var name in ingredients)
                {
                    var ing = await _context.Ingredients.FirstOrDefaultAsync(i => i.IngredientName.ToLower() == name.ToLower());
                    if (ing == null)
                    {
                        ing = new Ingredients { IngredientName = name };
                        _context.Ingredients.Add(ing);
                        await _context.SaveChangesAsync();
                    }

                    dish.DishIngredients.Add(new DishIngredients
                    {
                        Dishes = dish,
                        Ingredients = ing
                    });
                }
            }

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Блюдо добавлено", dish.DishID });
        }

[HttpPut("{id}")]
public async Task<IActionResult> UpdateDish(int id, [FromForm] DishInputModel model, IFormFile? images)
{
    var dish = await _context.Dishes
        .Include(d => d.DishIngredients)
        .FirstOrDefaultAsync(d => d.DishID == id);

    if (dish == null) return NotFound();

    dish.DishName = model.DishName;
    dish.Price = model.Price;

    // Исправление: приводим RemoveImage вручную
    bool removeImage = model.RemoveImage == "true" || model.RemoveImage == "on";

    if (removeImage && !string.IsNullOrEmpty(dish.DishImage))
    {
        var oldPath = Path.Combine(_env.WebRootPath, dish.DishImage.TrimStart('/'));
        if (System.IO.File.Exists(oldPath))
            System.IO.File.Delete(oldPath);

        dish.DishImage = null;
    }
    else if (images != null)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(images.FileName)}";
        var filePath = Path.Combine(_env.WebRootPath, "images", fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await images.CopyToAsync(stream);
        dish.DishImage = "/images/" + fileName;
    }


    // обновление ингредиентов
    _context.DishIngredients.RemoveRange(dish.DishIngredients);
    dish.DishIngredients = new List<DishIngredients>();

    var ingredients = model.IngredientNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    foreach (var name in ingredients)
    {
        var ing = await _context.Ingredients
            .FirstOrDefaultAsync(i => i.IngredientName.ToLower() == name.ToLower());

        if (ing == null)
        {
            ing = new Ingredients { IngredientName = name };
            _context.Ingredients.Add(ing);
            await _context.SaveChangesAsync();
        }

        dish.DishIngredients.Add(new DishIngredients
        {
            DishID = dish.DishID,
            IngredientID = ing.IngredientID
        });
    }

    await _context.SaveChangesAsync();
    return Ok(new { message = "Блюдо обновлено" });
}


        // ---------------- DELETE ----------------

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes
                                     .Include(d => d.DishIngredients)
                                     .FirstOrDefaultAsync(d => d.DishID == id);

            if (dish == null)
            {
                return NotFound(new { message = "Блюдо не найдено" });
            }

            // Удаляем связи ингредиентов
            _context.DishIngredients.RemoveRange(dish.DishIngredients);

            // Удаляем блюдо
            _context.Dishes.Remove(dish);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Блюдо успешно удалено" });
        }
    }

    // DTO
    public class DishInputModel
    {
        public string DishName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string IngredientNames { get; set; } = string.Empty;
        public string? RemoveImage { get; set; }
    }
}
