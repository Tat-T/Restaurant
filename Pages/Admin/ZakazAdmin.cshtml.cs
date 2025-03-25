using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

[Authorize]
public class ZakazAdminModel : PageModel
{
    public readonly AppDbContext _context;

    public ZakazAdminModel(AppDbContext context)
    {
        _context = context;
    }

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
