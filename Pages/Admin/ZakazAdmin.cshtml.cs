using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using Microsoft.AspNetCore.Identity;
using MyRazorApp.Models;

public class ZakazAdminModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public ZakazAdminModel(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Reservation Reservation { get; set; } = new();

    public List<Reservation> Reservations { get; set; } = new();

    public bool IsAdmin { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // определяем роль
        IsAdmin = User.IsInRole("Admin");

        if (IsAdmin)
        {
            Reservations = await _context.Reservations
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }
        else
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                // авто-заполнение формы
                Reservation.Name = currentUser.Name;
                Reservation.Email = currentUser.Email;
                Reservation.Phone = currentUser.PhoneNumber;
            }

            Reservations = await _context.Reservations
                .Where(r => r.Email == currentUser.Email)
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // если пользователь авторизован — подтянем email
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            Reservation.Email = currentUser.Email;
            Reservation.Name = string.IsNullOrWhiteSpace(Reservation.Name) ? currentUser.Name : Reservation.Name;
            Reservation.Phone = string.IsNullOrWhiteSpace(Reservation.Phone) ? currentUser.PhoneNumber : Reservation.Phone;
        }

        Reservation.ReservationDate = Reservation.ReservationDate.Date;

        _context.Reservations.Add(Reservation);
        await _context.SaveChangesAsync();

        return RedirectToPage(); // обновляем страницу после сохранения
    }
}
