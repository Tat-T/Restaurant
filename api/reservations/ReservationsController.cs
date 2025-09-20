using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRazorApp.Data;

namespace Restaurant.Api.Reservations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("slots")]
        public IActionResult GetSlots()
        {
            var today = DateTime.Today;

            var dates = Enumerable.Range(0, 7)
                                  .Select(i => today.AddDays(i).ToString("yyyy-MM-dd"))
                                  .ToList();

            var times = new List<string>
            {
                "12:00","13:00","14:00","15:00",
                "16:00","17:00","18:00","19:00",
                "20:00","21:00","22:00"
            };

            var booked = _context.Reservations
                                 .Where(r => r.ReservationDate >= today)
                                 .GroupBy(r => r.ReservationDate.ToString("yyyy-MM-dd"))
                                 .ToDictionary(
                                     g => g.Key,
                                     g => g.Select(r => r.ReservationTime.ToString(@"hh\:mm")).ToList()
                                 );

            return Ok(new { availableDates = dates, availableTimes = times, bookedSlots = booked });
        }

        [HttpPost]
        public IActionResult Create([FromBody] Reservation reservation)
        {
            if (reservation == null)
                return BadRequest(new { message = "Некорректные данные" });

            var exists = _context.Reservations.Any(r =>
                r.ReservationDate == reservation.ReservationDate &&
                r.ReservationTime == reservation.ReservationTime
            );

            if (exists)
                return Conflict(new { message = "Этот слот уже забронирован" });

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return Ok(new { message = "Бронирование успешно" });
        }
    }
}
