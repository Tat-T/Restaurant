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
        public Reservation Reservation { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Reservations.Add(Reservation);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/ZakazAdmin");
        }
}