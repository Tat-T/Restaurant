using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace Restaurant.Api.Reservations
{
  [Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public ReservationsController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("slots")]
    public async Task<IActionResult> GetSlots()
    {
        var today = DateTime.Today;

        var dates = Enumerable.Range(0, 7)
                              .Select(i => today.AddDays(i).ToString("yyyy-MM-dd"))
                              .ToList();

        var times = Enumerable.Range(12, 11)
                              .Select(h => new TimeSpan(h, 0, 0).ToString(@"hh\:mm"))
                              .ToList();

        var reservations = await _context.Reservations
                                         .Where(r => r.ReservationDate >= today)
                                         .ToListAsync();

        var booked = reservations
                     .GroupBy(r => r.ReservationDate.ToString("yyyy-MM-dd"))
                     .ToDictionary(
                         g => g.Key,
                         g => g.Select(r => r.ReservationTime.ToString(@"hh\:mm")).ToList()
                     );

        // Данные авторизованного пользователя
        User? user = null;
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            user = await _userManager.GetUserAsync(User);
        }

        return Ok(new { availableDates = dates, availableTimes = times, bookedSlots = booked, user });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Reservation reservation)
    {
        if (reservation == null)
            return BadRequest(new { message = "Некорректные данные" });

        bool exists = await _context.Reservations.AnyAsync(r =>
            r.ReservationDate == reservation.ReservationDate &&
            r.ReservationTime == reservation.ReservationTime
        );

        if (exists)
            return Conflict(new { message = "Этот слот уже забронирован" });

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Бронирование успешно" });
    }
}

}
