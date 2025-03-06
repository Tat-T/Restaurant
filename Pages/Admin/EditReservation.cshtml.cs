using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

[Authorize(Roles = "Admin")]
public class EditReservationModel : PageModel
{
    private readonly AppDbContext _context;

    public EditReservationModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Reservation? Reservation { get; set; } = new();  // ✅ Инициализация

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Reservation = await _context.Reservations.FindAsync(id);

        if (Reservation == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Reservation == null)  // ✅ Проверка на null
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Reservation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Reservations.Any(e => e.Id == Reservation.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("/Account/Index");
    }
}