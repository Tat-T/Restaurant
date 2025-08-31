using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Data;
using MyRazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyRazorApp.Pages
{
    public class ZakazModel : PageModel
    {
        private readonly AppDbContext _context;

        public ZakazModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = new Reservation();

        public List<DateTime> AvailableDates { get; set; } = new();
        public List<DateTime> BookedDates { get; set; } = new();
        public List<TimeSpan> AvailableTimes { get; set; } = new();
        public List<TimeSpan> BookedTimes { get; set; } = new();

        public Dictionary<string, List<string>> BookedSlots { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Доступные даты (например, ближайшие 7 дней)
            AvailableDates = Enumerable.Range(0, 7)
                .Select(offset => DateTime.Today.AddDays(offset))
                .ToList();

            // Доступные часы (12:00 – 22:00)
            AvailableTimes = Enumerable.Range(12, 10)
                .Select(h => new TimeSpan(h, 0, 0))
                .ToList();

            // Загружаем брони из БД
            var reservations = await _context.Reservations.ToListAsync();

            BookedSlots = reservations
                .GroupBy(r => r.ReservationDate.Date)
                .ToDictionary(
                    g => g.Key.ToString("yyyy-MM-dd"), // ключ — дата в строке
                    g => g.Select(r => r.ReservationTime.ToString(@"hh\:mm")).ToList()
                );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            _context.Reservations.Add(Reservation);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Success");
        }
    }
}
