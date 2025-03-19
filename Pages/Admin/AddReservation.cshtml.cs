using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

[Authorize(Roles = "Admin")]
public class AddReservationModel : PageModel
{
    private readonly AppDbContext _context;

    public AddReservationModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Reservation? Reservation { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Reservation = await _context.Reservations.FindAsync(id);

        if (Reservation == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Reservation newReservation)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Reservations.Add(newReservation);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Account/Index");
    }
}