using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

namespace Restaurant.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminReservationsController> _logger;
        private readonly IWebHostEnvironment _env;

        public AdminReservationsController(AppDbContext context,
                                   ILogger<AdminReservationsController> logger,
                                   IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        // Получение конкретного бронирования по ID + список доступных дат/времени
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Бронирование не найдено" });

            var today = DateTime.Today;

            var availableDates = Enumerable.Range(0, 7)
                                           .Select(offset => today.AddDays(offset).ToString("yyyy-MM-dd"))
                                           .ToList();

            var availableTimes = Enumerable.Range(12, 11) // 12:00–22:00
                                           .Select(h => new TimeSpan(h, 0, 0).ToString(@"hh\:mm"))
                                           .ToList();

            var reservations = await _context.Reservations.ToListAsync();

            var bookedSlots = reservations
                .GroupBy(r => r.ReservationDate.ToString("yyyy-MM-dd"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.ReservationTime.ToString(@"hh\:mm")).ToList()
                );

            return Ok(new
            {
                reservation,
                availableDates,
                availableTimes,
                bookedSlots
            });
        }

        // Редактирование бронирования
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation updated)
        {
            if (id != updated.Id)
                return BadRequest(new { message = "ID в запросе и объекте не совпадают" });

            var exists = await _context.Reservations.AnyAsync(r =>
                r.Id != id &&
                r.ReservationDate == updated.ReservationDate &&
                r.ReservationTime == updated.ReservationTime
            );

            if (exists)
                return Conflict(new { message = "Этот слот уже забронирован" });

            _context.Entry(updated).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Reservations.AnyAsync(r => r.Id == id))
                    return NotFound(new { message = "Бронирование не найдено" });
                throw;
            }

            return Ok(new { message = "Бронирование успешно обновлено" });
        }

        // Добавление нового бронирования
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation newReservation)
        {
            var exists = await _context.Reservations.AnyAsync(r =>
                r.ReservationDate == newReservation.ReservationDate &&
                r.ReservationTime == newReservation.ReservationTime
            );

            if (exists)
                return Conflict(new { message = "Этот слот уже забронирован" });

            _context.Reservations.Add(newReservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = newReservation.Id }, newReservation);
        }

         // DELETE: api/AdminReservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { message = "Бронирование не найдено" });
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
