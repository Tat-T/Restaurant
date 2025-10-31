using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace Restaurant.Api
{
    // [Authorize]
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _context
                .Reservations.OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(reservations);
        }

        [HttpGet("slots")]
        public async Task<IActionResult> GetSlots()
        {
            var today = DateTime.Today;

            var dates = Enumerable
                .Range(0, 7)
                .Select(i => today.AddDays(i).ToString("yyyy-MM-dd"))
                .ToList();

            var times = Enumerable
                .Range(12, 11)
                .Select(h => new TimeSpan(h, 0, 0).ToString(@"hh\:mm"))
                .ToList();

            var reservations = await _context
                .Reservations.Where(r => r.ReservationDate >= today)
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

            return Ok(
                new
                {
                    availableDates = dates,
                    availableTimes = times,
                    bookedSlots = booked,
                    user,
                }
            );
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Reservation reservation)
        {
            if (reservation == null)
                return BadRequest(new { message = "Некорректные данные" });

            bool exists = await _context.Reservations.AnyAsync(r =>
                r.ReservationDate == reservation.ReservationDate
                && r.ReservationTime == reservation.ReservationTime
            );

            if (exists)
                return Conflict(new { message = "Этот слот уже забронирован" });

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Бронирование успешно" });
        }

        // Получение конкретного бронирования по ID + список доступных дат/времени
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Бронирование не найдено" });

            var today = DateTime.Today;

            var availableDates = Enumerable
                .Range(0, 7)
                .Select(offset => today.AddDays(offset).ToString("yyyy-MM-dd"))
                .ToList();

            var availableTimes = Enumerable
                .Range(12, 11) // 12:00–22:00
                .Select(h => new TimeSpan(h, 0, 0).ToString(@"hh\:mm"))
                .ToList();

            var reservations = await _context.Reservations.ToListAsync();

            var bookedSlots = reservations
                .GroupBy(r => r.ReservationDate.ToString("yyyy-MM-dd"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.ReservationTime.ToString(@"hh\:mm")).ToList()
                );

            return Ok(
                new
                {
                    reservation,
                    availableDates,
                    availableTimes,
                    bookedSlots,
                }
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found" });
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Reservation updated)
        {
            if (updated == null || id != updated.Id)
                return BadRequest(new { message = "Некорректные данные" });

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Бронирование не найдено" });

            // Обновляем поля
            reservation.Name = updated.Name;
            reservation.Email = updated.Email;
            reservation.Phone = updated.Phone;
            reservation.ReservationDate = updated.ReservationDate;
            reservation.ReservationTime = updated.ReservationTime;
            reservation.Guests = updated.Guests;
            reservation.Message = updated.Message;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Бронирование обновлено" });
        }
    }
}
