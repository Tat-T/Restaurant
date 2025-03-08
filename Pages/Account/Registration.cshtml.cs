using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

public class RegistrationModel : PageModel
{
    private readonly AppDbContext _context;

    public RegistrationModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public required new Users User { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Проверяем, не существует ли уже такой пользователь
        var existingUser = await _context.Users.AnyAsync(u => u.Login == User.Login || u.Email == User.Email);
        if (existingUser)
        {
            ModelState.AddModelError("", "Пользователь с таким логином или email уже существует.");
            return Page();
        }

        User.CreationDate = DateTime.Now;
        User.IdRole = 5;
        User.IsActive = true; // Активируем пользователя по умолчанию

        _context.Users.Add(User);
        await _context.SaveChangesAsync();

        // Создаем Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, User.Email),
            new Claim("UserId", User.Id.ToString()),
            new Claim("IsActive", User.IsActive.ToString()),  // Добавляем IsActive в Claims
            new Claim(ClaimTypes.Role, User.IdRole.ToString()) // Сохраняем роль
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Авторизуем пользователя
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return RedirectToPage("/Zakaz");
    }
}
