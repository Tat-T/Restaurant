using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Pages.Admin
{
    public class AddMenuModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AddMenuModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public DishInputModel DishInput { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadImage { get; set; }

        public string? StatusMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            string imagePath = "/image/no-photo.jpg";

            if (UploadImage != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(UploadImage.FileName)}";
                var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadImage.CopyToAsync(stream);
                }

                imagePath = "/images/" + fileName;
            }

            var dish = new Dishes
            {
                DishName = DishInput.DishName,
                Price = DishInput.Price,
                DishImage = imagePath,
                DishIngredients = new List<DishIngredients>()
            };

            // разбор ингредиентов
            if (!string.IsNullOrWhiteSpace(DishInput.IngredientNames))
            {
                var ingredients = DishInput.IngredientNames
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Trim());

                foreach (var name in ingredients)
                {
                    // ищем ингредиент в БД
                    var existingIngredient = _context.Ingredients
                        .FirstOrDefault(x => x.IngredientName == name);

                    if (existingIngredient == null)
                    {
                        existingIngredient = new Ingredients { IngredientName = name };
                        _context.Ingredients.Add(existingIngredient);
                    }

                    dish.DishIngredients.Add(new DishIngredients
                    {
                        Dishes = dish,
                        Ingredients = existingIngredient
                    });
                }
            }

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            StatusMessage = "Блюдо успешно добавлено!";
            return RedirectToPage("/Admin/MenuAdmin");
        }

        public class DishInputModel
        {
            [Required]
            public string DishName { get; set; } = string.Empty;

            [Required]
            [Range(1, 10000)]
            public decimal Price { get; set; }

            public string? DishImage { get; set; }

            // Ингредиенты вводим через запятую
            public string? IngredientNames { get; set; }
        }
    }
}
