using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

[Authorize]
public class Index2Model : PageModel
{
    public readonly AppDbContext _context;

    public Index2Model(AppDbContext context)
    {
        _context = context;
    }
    public List<DishViewModel> Dishes { get; set; } = new ();

    public List<Reservation> Reservations { get; set; } = new ();

    public async Task<IActionResult> OnGetAsync()
    {
        // Получаем email и роль текущего пользователя
        var userEmail = User.FindFirstValue(ClaimTypes.Name);
        var userRole = User.FindFirstValue(ClaimTypes.Role); // Админ или Пользователь

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToPage("/Account/Login");
        }

        // Если админ — получаем все заказы, иначе только заказы текущего пользователя
        if (userRole == "Admin")
        {
            Reservations = await _context.Reservations.ToListAsync();
            Dishes = await _context.Dishes
            .Select(d => new DishViewModel
            {
                DishID = d.DishID,
                DishName = d.DishName,
                Price = d.Price,
                // DishImage = "/image/dishes/" + d.DishID + ".jpg",
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
        }
        else
        {
            Reservations = await _context.Reservations
                .Where(r => r.Email == userEmail)
                .ToListAsync();
        }

        return Page();
    }
}
