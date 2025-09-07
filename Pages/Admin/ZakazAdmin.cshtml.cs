using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

// [Authorize]
public class ZakazAdminModel : PageModel
{
    private readonly AppDbContext _context;

    public ZakazAdminModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Reservation> Reservations { get; set; } = new();

    public bool IsAdmin { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        IsAdmin = !string.IsNullOrEmpty(userRole) &&
                  string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);

        if (IsAdmin)
        {
            Reservations = await _context.Reservations
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }
        else
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";

            Reservations = await _context.Reservations
                .Where(r => r.Email == userEmail)
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        return Page();
    }
}
