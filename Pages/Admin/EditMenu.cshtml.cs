using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using System.ComponentModel.DataAnnotations;

[Authorize(Roles = "Admin")]
public class EditMenuModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public EditMenuModel(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public DishInputModel DishInput { get; set; } = new();

    [BindProperty]
    public IFormFile? UploadImage { get; set; } // Файл для загрузки

    [BindProperty]
    public bool RemoveImage { get; set; } // Флаг удаления картинки

    [TempData]
    public string? StatusMessage { get; set; }

    public class DishInputModel
    {
        public int DishID { get; set; }

        [Required]
        [StringLength(255)]
        public string DishName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        public string? DishImage { get; set; }


        [Required(ErrorMessage = "Укажите ингредиенты через запятую")]
        public string IngredientNames { get; set; } = "";
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var dish = await _context.Dishes
            .Include(d => d.DishIngredients)
            .ThenInclude(di => di.Ingredients)
            .FirstOrDefaultAsync(d => d.DishID == id);

        if (dish == null)
        {
            return NotFound();
        }

        DishInput = new DishInputModel
        {
            DishID = dish.DishID,
            DishName = dish.DishName,
            Price = dish.Price,
            DishImage = dish.DishImage,
            IngredientNames = string.Join(", ", dish.DishIngredients.Select(di => di.Ingredients.IngredientName))
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dish = await _context.Dishes
            .Include(d => d.DishIngredients)
            .FirstOrDefaultAsync(d => d.DishID == DishInput.DishID);

        if (dish == null)
        {
            return NotFound();
        }

        // Обновляем данные блюда
        dish.DishName = DishInput.DishName;
        dish.Price = DishInput.Price;

         // Удаление картинки
        if (RemoveImage && !string.IsNullOrEmpty(dish.DishImage))
        {
            var filePath = Path.Combine(_env.WebRootPath, dish.DishImage.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            dish.DishImage = null;
        }
        else if (UploadImage != null && UploadImage.Length > 0)
        {
            // Загрузка новой картинки
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(UploadImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await UploadImage.CopyToAsync(stream);
            }

            dish.DishImage = "/images/" + uniqueFileName;
        }
        else
        {
            // Если ничего не меняли — оставляем прежнюю
            dish.DishImage = DishInput.DishImage;
        }

        // Удаляем старые связи ингредиентов
        _context.DishIngredients.RemoveRange(dish.DishIngredients);

        // Обработка ингредиентов
        var ingredientNames = DishInput.IngredientNames
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var newDishIngredients = new List<DishIngredients>();

        foreach (var name in ingredientNames)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.IngredientName.ToLower() == name.ToLower());

            if (ingredient == null)
            {
                ingredient = new Ingredients { IngredientName = name };
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
            }

            newDishIngredients.Add(new DishIngredients
            {
                DishID = dish.DishID,
                IngredientID = ingredient.IngredientID
            });
        }

        dish.DishIngredients = newDishIngredients;

        await _context.SaveChangesAsync();
        StatusMessage = "Блюдо обновлено успешно!";
        return RedirectToPage("/Admin/MenuAdmin");
    }
}