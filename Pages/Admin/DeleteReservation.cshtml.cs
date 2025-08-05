using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

[Authorize(Roles = "Admin")]
public class DeleteReservationModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteReservationModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Reservation? Reservation { get; set; }

    // Получаем данные бронирования для подтверждения удаления
    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        Reservation = await _context.Reservations
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(r => r.Id == id);

        if (Reservation == null)
        {
            return NotFound();
        }

        return Page();
    }

    // Удаляем бронирование
    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Admin/ZakazAdmin");
    }
}
