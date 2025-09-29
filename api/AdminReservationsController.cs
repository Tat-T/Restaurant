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

        public AdminReservationsController(AppDbContext context)
        {
            _context = context;
        }

        // Получение конкретного бронирования по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            var today = DateTime.Today;

            var availableDates = Enumerable.Range(0, 7)
                                           .Select(offset => today.AddDays(offset))
                                           .ToList();

            var availableTimes = Enumerable.Range(12, 11)
                                           .Select(h => new TimeSpan(h, 0, 0))
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
                availableDates = availableDates.Select(d => d.ToString("yyyy-MM-dd")),
                availableTimes = availableTimes.Select(t => t.ToString(@"hh\:mm")),
                bookedSlots
            });
        }


        // Редактирование бронирования
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation updated)
        {
            if (id != updated.Id) return BadRequest();

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
                    return NotFound();
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

            return Ok(new { message = "Бронирование успешно создано" });
        }
        
        // Удаление бронирования
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
    
            return Ok(new { message = "Бронирование успешно удалено" });
        }
        }
}
