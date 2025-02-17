using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;


public class Index2Model : PageModel
{
    public readonly AppDbContext _context;

    public Index2Model(AppDbContext context)
    {
        _context = context;
    }

    public List<Reservation> Reservations { get; set; } = new ();

    public async Task OnGetAsync()
    {
        Reservations = await _context.Reservations.ToListAsync();
    }
}
